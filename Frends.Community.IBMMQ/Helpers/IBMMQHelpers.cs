using System;
using System.Collections;
using System.Transactions;
using IBM.WMQ;
using System.Text;

#pragma warning disable 1591

namespace Frends.Community.IBMMQ.Helpers
{
    public static class IBMMQHelpers
    {
        public static MQQueueManager CreateQMgrConnection(ConnectionParameters connection, TaskOptions options)
        {
            var connectionProperties = new Hashtable {
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED},
                { MQC.HOST_NAME_PROPERTY, connection.HostName },
                { MQC.PORT_PROPERTY, connection.PortNumber },
                { MQC.CHANNEL_PROPERTY, connection.Channel }  };

            if (options.SslCertStore != "") connectionProperties.Add(MQC.SSL_CERT_STORE_PROPERTY, options.SslCertStore);
            if (options.SslCipherSpec != "") connectionProperties.Add(MQC.SSL_CIPHER_SPEC_PROPERTY, options.SslCipherSpec);
            if (options.SslPeerName != "") connectionProperties.Add(MQC.SSL_PEER_NAME_PROPERTY, options.SslPeerName);
            if (options.SslResetCount != 0) connectionProperties.Add(MQC.SSL_RESET_COUNT_PROPERTY, options.SslResetCount);
            if (connection.UserId != "") connectionProperties.Add(MQC.USER_ID_PROPERTY, connection.UserId);
            if (connection.Password != "") connectionProperties.Add(MQC.PASSWORD_PROPERTY, connection.Password);
            if (options.SslCertRevocationCheck) MQEnvironment.SSLCertRevocationCheck = true;

            return new MQQueueManager(connection.QueueManagerName, connectionProperties);
        }

        /// <summary>
        ///     Helper method to set message properties (properties and descriptors).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageProperties"></param>
        /// <param name="messageAsBytes"></param>
        public static void SetMessagePropertiesAndHeaders(MQMessage message, PutMessageProperties messageProperties, bool messageAsBytes)
        {

            // Message descriptors (MQMD) 
            if (messageProperties.Descriptors.Length > 0)
                foreach (var kvp in messageProperties.Descriptors)
                {
                    message.MQMD.SetValue(kvp.Name, kvp.Value);
                }

            // Message properties
            if (messageProperties.Properties.Length > 0)
                foreach (var kvp in messageProperties.Properties)
                {
                    message.SetValue(kvp.Name, kvp.Value);
                }

            // Charset set only if message is sent as text
            // This overrides common Message Properties above if not set to Automatic
            if (!messageAsBytes && !(messageProperties.CharacterSet == CharacterSetEnum.Automatic))
            {
                if (messageProperties.CharacterSet == CharacterSetEnum.Other)
                {
                    if (messageProperties.CharacterSetValue != null)
                        message.CharacterSet = (int)messageProperties.CharacterSetValue;
                }
                else
                    message.CharacterSet = MapMessageCharacterSet(messageProperties.CharacterSet.ToString());
            }

            // Set RFH2 headers if desired
            if (messageProperties.SetRFH2Headers)
                WriteRFH2Header(message, messageProperties.RFH2Headers);
        }

        /// <summary>
        ///     Generic extension method to set object properties, 
        ///     used for instance to set MQ Message properties and decriptor values.
        ///     Code borrowed (with minor modifications) from: https://social.technet.microsoft.com/wiki/contents/articles/54296.dynamically-set-property-value-in-a-class-c.aspx
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetValue<T>(this T sender, string propertyName, object value)
        {
            var propertyInfo = sender.GetType().GetProperty(propertyName);

            if (propertyInfo is null) return;

            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(sender, Enum.Parse(propertyInfo.PropertyType, value.ToString()));
            }
            else
            {
                var safeValue = (value == null) ? null : Convert.ChangeType(value, type);
                propertyInfo.SetValue(sender, safeValue, null);
            }
        }

        private static int MapMessageCharacterSet(string characterSetEnumValue)
        {
            switch (characterSetEnumValue)
            {
                case "UTF8":
                    return 1208;
                case "ISO88591":
                    return 819;
                case "WindowsLatin1":
                    return 1252;
                case "Unicode":
                    return 1200;
                default:
                    // Default is unicode (1200)
                    return 1200;
            }
        }

        /// <summary>
        ///     Helper method to map transaction type to actual enumeration (TransactionScopeOption).
        /// </summary>
        /// <param name="transactionTypeEnumValue"></param>
        /// <returns>TransactionScopeOption enumeration value</returns>
        public static TransactionScopeOption MapTransctionType(string transactionTypeEnumValue)
        {
            switch (transactionTypeEnumValue)
            {
                case "Required":
                    return TransactionScopeOption.Required;
                case "RequiresNew":
                    return TransactionScopeOption.RequiresNew;
                default:
                    // Default is Required
                    return TransactionScopeOption.Required;
            }
        }


        #region MQ RFH2 headers handling

        /// <summary>
        ///     Method to check if there is an RFH2 header in the message.
        ///     If there is, parse and remove it and return object containing 
        ///     the header information.
        /// </summary>
        /// <param name="message">MQMessage to read RFH2 Headers from</param>
        /// <returns>Object containing RFH2 headers</returns>
        public static MessageAndRFH2Header ReadAndStripRFH2Header(MQMessage message)
        {
            MessageAndRFH2Header result = new MessageAndRFH2Header();

            // Little endian is the default
            bool bigEndian = false;

            RFH2Headers rfh = new RFH2Headers
            {
                StrucId = Encoding.UTF8.GetString(message.ReadBytes(4))
            };

            // Check if start of message contains RFH header data
            // If not, then reset to the beginning of the message and return
            if (!rfh.StrucId.Trim().Contains("RFH"))
            {
                result.RFH2HeadersFound = false;
                result.Headers = rfh;

                // Reset message to the beginning of the message
                message.Seek(0);

                return result;
            }

            rfh.Version = BitConverter.ToInt32(message.ReadBytes(4), 0);
            // We can deduct endianess from the Version (don't know how/why, this is how it was made where this was migrated from)
            // NB: Big Endian is not tested at all, only Little Endian works
            if (rfh.Version > 2 || rfh.Version < 1)
            {
                rfh.Version = SwapBytes(rfh.Version);

                // Other integer values are swapped if this is set to true
                bigEndian = true;
            }

            rfh.StrucLength = BitConverter.ToInt32(message.ReadBytes(4), 0);
            if (bigEndian)
                rfh.StrucLength = SwapBytes(rfh.StrucLength);

            rfh.Encoding = BitConverter.ToInt32(message.ReadBytes(4), 0);
            if (bigEndian)
                rfh.Encoding = SwapBytes(rfh.Encoding);

            rfh.CodedCharSetId = BitConverter.ToInt32(message.ReadBytes(4), 0);
            if (bigEndian)
                rfh.CodedCharSetId = SwapBytes(rfh.CodedCharSetId);

            rfh.Format = Encoding.UTF8.GetString(message.ReadBytes(8));
            rfh.Flags = BitConverter.ToInt32(message.ReadBytes(4), 0);
            if (bigEndian)
                rfh.Flags = SwapBytes(rfh.Flags);

            rfh.NameValueCCSID = BitConverter.ToInt32(message.ReadBytes(4), 0);
            if (bigEndian)
                rfh.NameValueCCSID = SwapBytes(rfh.NameValueCCSID);

            var data = new StringBuilder();
            // we have read 36 bytes at this point
            int bytesLeft = rfh.StrucLength - 36;

            // there migth be multiple lenght-data -pairs (or none)
            // combine all to a single data field
            while (bytesLeft > 0)
            {
                int dataPartLength = BitConverter.ToInt32(message.ReadBytes(4), 0);
                if (bigEndian)
                    dataPartLength = SwapBytes(dataPartLength);

                string dataPart = Encoding.UTF8.GetString(message.ReadBytes(dataPartLength));
                data.Append(dataPart);

                bytesLeft = bytesLeft - 4 - dataPartLength;
            }
            rfh.NameValueData = data.ToString();
            rfh.NameValueLength = Encoding.UTF8.GetBytes(data.ToString()).Length;

            result.RFH2HeadersFound = true;
            result.Headers = rfh;

            return result;
        }

        /// <summary>
        /// Method to write RFH2 headers to a MQ Message.
        /// Allows only one NameValueLength/Data pair to be written.
        /// 
        /// Offers setting headers values individually, default values are used for headers not explicitly given.
        /// </summary>
        /// <param name="message">MQ Message headers are written to</param>
        /// <param name="rFH2Headers">RFH2 Headers</param>
        /// <param name="bigEndian">true if byte order is big endian</param>
        /// <returns></returns>
        public static void WriteRFH2Header(MQMessage message, RFH2HeaderProperty[] rFH2Headers, bool bigEndian = false)
        {
            var headers = new RFH2Headers();

            foreach (var kvp in rFH2Headers)
            {
                // Do not use the StrucLenght if given
                // Use the default value + calculated (below)
                if (kvp.Name != "StrucLength")
                    headers.SetValue(kvp.Name, kvp.Value);
            }

            // Reset message's DataOffset to zero just in case
            // This may have been set beforehand by setting MessageProperty values
            message.DataOffset = 0;

            message.WriteString(headers.StrucId.PadRight(4, ' '));
            message.WriteInt4(bigEndian ? SwapBytes(headers.Version) : headers.Version);

            // StrucLength need to be recalculated
            // and it should be a multiply of 4
            // Get the length for the text as byte length and string length
            // (some characters require two bytes but are calculated as one char in string length)
            int lenStr = headers.NameValueData.Length;
            int lenByte = Encoding.UTF8.GetBytes(headers.NameValueData).Length;
            int remainderByte = lenByte % 4;

            int lenByteCorrected = lenByte;
            if (remainderByte != 0)
            {
                lenByteCorrected = lenByteCorrected + 4 - remainderByte;
            }

            // If there are character that are represented as two bytes we need
            // come calculations in order to be able to read the headers correctly
            // Eg. ä, Ä, ö, Ö chracters require two bytes
            // Pad string data to match the byte length of the nearest multiply of 4
            // 
            // Examples
            // - "AA"   -> "AA  "   (two 1-byte chars + two whitepace = 4 bytes)
            // - "AÖ"   -> "AÖ "    (one 1-byte char + one 2-byte char + one whitespace = 4 bytes)
            // - "ÄÄ"   -> "ÄÄ"     (two 2-byte chars = 4 bytes)
            // - "ÖÖÖ"  -> "ÖÖÖ  "  (three 2-byte chars + two whitespace = 8 bytes)
            headers.NameValueData = headers.NameValueData.PadRight(lenByteCorrected - lenByte + lenStr);
            // Add the length of the StrucLength also (4 bytes) (only if NameValueData contains data)
            if (lenByteCorrected > 0)
                headers.StrucLength += lenByteCorrected + 4;

            message.WriteInt4(bigEndian ? SwapBytes(headers.StrucLength) : headers.StrucLength);
            message.WriteInt4(bigEndian ? SwapBytes(headers.Encoding) : headers.Encoding); // BigEndian: 273, LittleEndian: 546
            message.WriteInt4(bigEndian ? SwapBytes(headers.CodedCharSetId) : headers.CodedCharSetId);
            message.WriteString(headers.Format.PadRight(8, ' '));
            message.WriteInt4(bigEndian ? SwapBytes(headers.Flags) : headers.Flags);
            message.WriteInt4(bigEndian ? SwapBytes(headers.NameValueCCSID) : headers.NameValueCCSID);

            // Write NameValueLength and Data only if Data contains data
            if (lenByteCorrected > 0)
            {
                message.WriteInt4(bigEndian ? SwapBytes(lenByteCorrected) : lenByteCorrected);
                message.WriteString(headers.NameValueData);
            }
        }

        /// <summary>
        /// Helper method to swap byte order of an integer (Little vs Big endian)
        /// </summary>
        /// <param name="number">int</param>
        /// <returns>int</returns>
        private static int SwapBytes(int number)
        {
            byte[] buffer = BitConverter.GetBytes(number);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        #endregion
    }
}