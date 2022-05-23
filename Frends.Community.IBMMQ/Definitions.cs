using System;
using System.Collections.Generic;
using IBM.WMQ;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frends.Community.IBMMQ.Helpers;

#pragma warning disable 1591

namespace Frends.Community.IBMMQ
{
    #region Task input parameters

    public enum TransactionType
    {
        Required,
        RequiresNew
    }

    public class GetQueueStatusInput
    {
        /// <summary>
        /// Queue names
        /// </summary>
        [DefaultValue("DEV.QUEUE.1")]
        [DisplayFormat(DataFormatString = "Text")]
        public string[] Queues { get; set; }
    }

    public class GetMessagesTaskParameters
    {

        /// <summary>
        /// Queue name to put message to
        /// </summary>
        //[UIHint(nameof(SourceType), "", QueueOrTopicType.Queue)]
        [DefaultValue("DEV.QUEUE.1")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Queue { get; set; }

        /// <summary>
        /// Maximum number of messages to get
        /// </summary>
        [DefaultValue(1)]
        public int MessageCount { get; set; }

        /// <summary>
        /// If set to true, message contents are returned as a byte array. 
        /// Return field name is MessageBytes instead of Message
        /// </summary>
        [DefaultValue(false)]
        public bool MessageAsBytes { get; set; }

        /// <summary>
        /// Return Message properties in addition to the actual message
        /// </summary>
        [DefaultValue(true)]
        public bool GetMessageProperties { get; set; }

        /// <summary>
        /// Return Message descriptor information in addition to the actual message
        /// </summary>
        [DefaultValue(false)]
        public bool GetMessageDescriptor { get; set; }

        /// <summary>
        /// Return and strip RFH2 headers from the beginning of the message so that they won't be seen in the actual message.
        /// RFH2 headers may be present in the message if the sender has written them.
        /// 
        /// This can be set to true with no ill effect even if the message doesn't have RFH2 headers.
        /// </summary>
        [DefaultValue(false)]
        public bool ParseRFH2Header { get; set; }

        /// <summary>
        /// Use transaction when getting messages
        /// 
        /// If set to Yes, when getting multiple messages one by one from the queue they are rolled back in case
        /// there is an error or the task is cancelled.
        /// </summary>
        [DefaultValue(true)]
        public bool UseTransaction { get; set; }

        /// <summary>
        /// Transaction type used.
        /// Default is Required which means an ambient transaction (current thread's transaction) is used if present and new transaction created if not.
        /// </summary>
        [UIHint(nameof(UseTransaction), "", true)]
        [DefaultValue(TransactionType.Required)]
        public TransactionType TransactionType { get; set; }

        /// <summary>
        /// Transaction timeout used, default is 60 seconds.
        /// </summary>
        [UIHint(nameof(UseTransaction), "", true)]
        [DefaultValue(60)]
        public int TransactionTimeout { get; set; }

#if DEBUG
        /// <summary>
        /// If using a transaction when getting messages
        /// sleep for 1s between each message if this is set to Yes.
        /// </summary>
        [UIHint(nameof(UseTransaction), "", true)]
        [DefaultValue(false)]
        public bool DebugTransaction { get; set; }
#endif
    }

    public class PeekMessageTaskParameters
    {
        /// <summary>
        /// Queue name to peek message from
        /// </summary>
        //[UIHint(nameof(SourceType), "", QueueOrTopicType.Queue)]
        [DefaultValue("DEV.QUEUE.1")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Queue { get; set; }

        /// <summary>
        /// If set to true, message contents are returned as a byte array. 
        /// Return field name is MessageBytes instead of Message
        /// </summary>
        [DefaultValue(false)]
        public bool MessageAsBytes { get; set; }

        /// <summary>
        /// Return Message properties in addition to the actual message
        /// </summary>
        [DefaultValue(true)]
        public bool GetMessageProperties { get; set; }

        /// <summary>
        /// Return Message descriptor information in addition to the actual message
        /// </summary>
        [DefaultValue(false)]
        public bool GetMessageDescriptor { get; set; }

        /// <summary>
        /// Return and strip RFH2 headers from the beginning of the message so that they won't be seen in the actual message.
        /// RFH2 headers may be present in the message if the sender has written them.
        /// 
        /// This can be set to true with no ill effect even if the message doesn't have RFH2 headers.
        /// </summary>
        [DefaultValue(false)]
        public bool ParseRFH2Header { get; set; }
    }

    public class PutMessageInputParameters
    {
        /// <summary>
        /// Queue name to put message to
        /// </summary>
        [DefaultValue("DEV.QUEUE.1")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Queue { get; set; }

        /// <summary>
        /// If set to true, message contents are returned as a byte array. 
        /// </summary>
        [DefaultValue(false)]
        public bool MessageAsBytes { get; set; }

        /// <summary>
        /// Message content as byte array
        /// </summary>
        [UIHint(nameof(MessageAsBytes), "", true)]
        [DisplayFormat(DataFormatString = "Expression")]
        public byte[] MessageContentBytes { get; set; }

        /// <summary>
        /// Message content as text
        /// </summary>
        [UIHint(nameof(MessageAsBytes), "", false)]
        [DisplayFormat(DataFormatString = "Expression")]
        public string MessageContent { get; set; }
    }

    public class ConnectionParameters
    {
        /// <summary>
        /// Host name
        /// </summary>
        [DefaultValue("localhost")]
        public string HostName { get; set; }

        /// <summary>
        /// Port number
        /// </summary>
        [DefaultValue(1414)]
        public int PortNumber { get; set; }

        /// <summary>
        /// MQ Channel to be used
        /// </summary>
        [DefaultValue("SYSTEM.DEF.SVRCONN")]
        public string Channel { get; set; }

        /// <summary>
        /// Queue manager name
        /// </summary>
        public string QueueManagerName { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [PasswordPropertyText]
        public string Password { get; set; }

    }

    public class TaskOptions
    {
        /// <summary>
        /// SSL Certificate store
        /// </summary>
        public string SslCertStore { get; set; }

        /// <summary>
        /// SSL Cipher spec
        /// </summary>
        public string SslCipherSpec { get; set; }

        /// <summary>
        /// SSL Peer name
        /// </summary>
        public string SslPeerName { get; set; }

        /// <summary>
        /// SSL Reset count
        /// </summary>
        public int SslResetCount { get; set; }

        /// <summary>
        /// SSL Certification Revocation check used
        /// </summary>
        public bool SslCertRevocationCheck { get; set; }
    }

    public class PutMessageProperties
    {
        /// <summary>
        /// Character set for the message.
        /// 
        /// Use Other to use values not listed.
        /// 
        /// Used ONLY if messages is sent as text (not as bytes).
        /// </summary>
        [DefaultValue(CharacterSetEnum.UTF8)]
        [DisplayName("Character set")]
        public CharacterSetEnum CharacterSet { get; set; }

        /// <summary>
        /// Character set for the message as integer code value.
        /// Integer values in IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-character-set-identifiers-net-applications
        /// </summary>
        [UIHint(nameof(CharacterSet), "", CharacterSetEnum.Other)]
        [DisplayName(@"Character set value")]
        [DisplayFormat(DataFormatString = "Expression")]
        public int? CharacterSetValue { get; set; }

        /// <summary>
        /// Individual message properties. See fields and types from IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=interfaces-mqmessagenet-class
        /// 
        /// Some values are derived and cannot be set explicitly.
        /// </summary>
        //[UIHint(nameof(UseMQMessageProperties), "", false)]
        public MessageProperty[] Properties { get; set; }

        /// <summary>
        /// Message descriptors (MQMD) for the message. See fields and types from IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=descriptor-fields-mqmd
        /// 
        /// Some values are derived and cannot be set explicitly.
        /// </summary>
        //[UIHint(nameof(UseMQMessageProperties), "", false)]
        public MessageDescriptorProperty[] Descriptors { get; set; }

        /// <summary>
        /// Set this to true, if RFH2 headers are required to be written to the beginning of the actual message.
        /// </summary>
        [DefaultValue(false)]
        public bool SetRFH2Headers { get; set; }

        /// <summary>
        /// You can set individual values or leave empty when default values are used. NB: Only one NameValueData field is allowed.
        /// Header names, types and default values from IBM MQ Documentation: https://www.ibm.com/docs/en/ibm-mq/9.2?topic=2-initial-values-language-declarations-mqrfh2
        /// </summary>
        [UIHint(nameof(SetRFH2Headers), "", true)]
        public RFH2HeaderProperty[] RFH2Headers { get; set; }

    }

    public enum CharacterSetEnum
    {
        UTF8,
        ISO88591,
        WindowsLatin1,
        Unicode,
        Other,
        Automatic
    }

    #endregion

    #region Task output params

    /// <summary>
    /// Get queue status output holds certain queue information
    /// </summary>
    public class GetQueueStatusOutput
    {
        /// <summary>
        /// Name of the queue
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Number of messages currently in the queue
        /// </summary>
        public int MessageCount { get; set; }

        /// <summary>
        /// Queue description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Put timestamp of the message in front of the queue
        /// </summary>
        public DateTime? OldestMessageQueueTime { get; set; }
    }

    /// <summary>
    /// Put messsage output is simply an boolean indicator that the 
    /// put was successful. In practice false-value is never returned as an Exception is
    /// thrown in case on an error.
    /// </summary>
    public class PutMessageOutput
    {
        public bool Success { get; set; }
    }

    /// <summary>
    /// Get messages output holds a list of messages
    /// </summary>
    public class GetMessagesOutput
    {
        public List<QueueMessage> Data { get; set; } = new List<QueueMessage>();
    }

    /// <summary>
    /// Peek messages output holds a property indication whether a message was found
    /// and if yes the actual message.
    /// </summary>
    public class PeekMessageOutput
    {
        public bool MessageFound { get; set; }

        public QueueMessage Data { get; set; } = new QueueMessage();
    }

    #endregion


    public class MessageProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class MessageDescriptorProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

   
    public class RFH2HeaderProperty
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class RFH2Headers
    {
        /// <summary>
        /// IBM MQ type: MQLONG
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// IBM MQ type: MQLONG
        /// </summary>
        public int CodedCharSetId { get; set; }

        /// <summary>
        /// IBM MQ type: MQLONG
        /// </summary>
        public int Encoding { get; set; }

        /// <summary>
        /// IBM MQ type: MQLONG
        /// </summary>
        public int Flags { get; set; }
        
        /// <summary>
        /// Max length of 8 characters
        /// IBM MQ type: MQCHAR8
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// IBM MQ type: MQLONG
        /// </summary>
        public int NameValueCCSID { get; set; }

        /// <summary>
        /// IBM MQ type: MQCHARn
        /// Distinction to IBM MQ, here this field contains all possibly separate sections.
        /// NameValueLength indicates the length of this field
        /// </summary>
        public string NameValueData { get; set; }

        /// <summary>
        /// IBM MQ type: MQLONG
        /// Distinction to IBM MQ, here indicates the total length of all NameValueData-sections
        /// which are combined into one.
        /// </summary>
        public int NameValueLength { get; set; }

        /// <summary>
        /// Max length of 4 characters
        /// IBM MQ type: MQCHAR4
        /// Must have value of MQRFH_STRUC_ID (IBM MQ Constant)
        /// </summary>
        public string StrucId { get; set; }

        /// <summary>
        /// IBM MQ type: MQLONG
        /// Indicates the entire length of the RFH2 header
        /// </summary>
        public int StrucLength { get; set; }

        /// <summary>
        /// Constructor for the class
        /// Sets the default values for required sections
        /// </summary>
        public RFH2Headers()
        {
            // Set default values
            StrucId = MQC.MQRFH_STRUC_ID;
            Version = MQC.MQRFH_VERSION_2;
            StrucLength = MQC.MQRFH_STRUC_LENGTH_FIXED_2;
            Encoding = MQC.MQENC_NATIVE;
            CodedCharSetId = MQC.MQCCSI_INHERIT;
            Format = MQC.MQFMT_NONE;
            Flags = MQC.MQRFH_NONE;
            NameValueCCSID = 1208;
            NameValueData = "";
            NameValueLength = 0;
        }
    }

    /// <summary>
    /// Intermediate class to hold the extracted RFH2 headers
    /// </summary>
    public class MessageAndRFH2Header
    {
        public bool RFH2HeadersFound { get; set; }

        public RFH2Headers Headers { get; set; }
    }

    public class MessageProperties
    {
        public int TotalMessageLength { get; set; }

        public DateTime PutDateTime { get; set; }

        public int PutApplicationType { get; set; }

        public string PutApplicationName { get; set; }

        public int MessageSequenceNumber { get; set; }

        public string ApplicationOriginData { get; set; }

        public string ApplicationIdData { get; set; }

        public string UserId { get; set; }

        public int PropertyValidation { get; set; }

        public int OriginalLength { get; set; }

        public int MessageType { get; set; }

        public int MessageLength { get; set; }

        public byte[] MessageId { get; set; }

        public int MessageFlags { get; set; }

        public byte[] GroupId { get; set; }

        public int DataOffset { get; set; }

        public int DataLength { get; set; }

        public byte[] CorrelationId { get; set; }

        public int CharacterSet { get; set; }

        public int BackoutCount { get; set; }

        public string AccountingToken { get; set; }

        public int Version { get; set; }

        public int Report { get; set; }

        public string ReplyToQueueName { get; set; }

        public string ReplyToQueueManagerName { get; set; }

        public int Priority { get; set; }

        public int Persistence { get; set; }

        public int Offset { get; set; }

        public string Format { get; set; }

        public int Feedback { get; set; }

        public int Expiry { get; set; }

        public int Encoding { get; set; }
    }

    public class MessageDescriptor : MQMessageDescriptor
    {

    }

    public class QueueMessage
    {
        public string Message { get; set; }

        public byte[] MessageBytes { get; set; }

        public MessageProperties MessageProperties { get; set; }

        public MessageDescriptor MessageDescriptor { get; set; }

        public RFH2Headers RFH2Headers { get; set; }

        public static QueueMessage FromMqMessage(MQMessage mqMessage, bool asBytes, bool returnMQProperties = true, bool returnMQMD = false, bool returnAndStripRFH2Header = false)
        {
            var rfh2Result = new MessageAndRFH2Header() { RFH2HeadersFound = false };

            // Try to extract RFH2 headers only if requested
            if (returnAndStripRFH2Header)
                rfh2Result = IBMMQHelpers.ReadAndStripRFH2Header(mqMessage);

            return new QueueMessage
            {
                // Include properties only if required
                MessageProperties = !returnMQProperties ? null : new MessageProperties
                {
                    Encoding = mqMessage.Encoding,
                    Expiry = mqMessage.Expiry,
                    Feedback = mqMessage.Feedback,
                    Format = mqMessage.Format.Trim(),
                    Offset = mqMessage.Offset,
                    Persistence = mqMessage.Persistence,
                    Priority = mqMessage.Priority,
                    Report = mqMessage.Report,
                    Version = mqMessage.Version,
                    AccountingToken = mqMessage.AccountingToken,
                    BackoutCount = mqMessage.BackoutCount,
                    CharacterSet = mqMessage.CharacterSet,
                    CorrelationId = mqMessage.CorrelationId,
                    DataLength = mqMessage.DataLength,
                    DataOffset = mqMessage.DataOffset,
                    GroupId = mqMessage.GroupId,
                    MessageFlags = mqMessage.MessageFlags,
                    MessageId = mqMessage.MessageId,
                    MessageLength = mqMessage.MessageLength,
                    MessageType = mqMessage.MessageType,
                    OriginalLength = mqMessage.OriginalLength,
                    PropertyValidation = mqMessage.PropertyValidation,
                    UserId = mqMessage.UserId.Trim(),
                    ApplicationIdData = mqMessage.ApplicationIdData.Trim(),
                    ApplicationOriginData = mqMessage.ApplicationOriginData.Trim(),
                    MessageSequenceNumber = mqMessage.MessageSequenceNumber,
                    PutApplicationName = mqMessage.PutApplicationName.Trim(),
                    PutApplicationType = mqMessage.PutApplicationType,
                    PutDateTime = mqMessage.PutDateTime,
                    TotalMessageLength = mqMessage.TotalMessageLength,
                    ReplyToQueueName = mqMessage.ReplyToQueueName.Trim(),
                    ReplyToQueueManagerName = mqMessage.ReplyToQueueManagerName.Trim()
                },

                // Return MessageDescriptor only if requested
                MessageDescriptor = !returnMQMD ? null : new MessageDescriptor
                {
                    StructMQMD = mqMessage.MQMD.StructMQMD,
                    Version = mqMessage.MQMD.Version,
                    Encoding = mqMessage.MQMD.Encoding,
                    BackoutCount = mqMessage.MQMD.BackoutCount,
                    Ccsid = mqMessage.MQMD.Ccsid,
                    MsgId = mqMessage.MQMD.MsgId,
                    CorrelId = mqMessage.MQMD.CorrelId,
                    Report = mqMessage.MQMD.Report,
                    MsgType = mqMessage.MQMD.MsgType,
                    Expiry = mqMessage.MQMD.Expiry,
                    Feedback = mqMessage.MQMD.Feedback,
                    CodedCharacterSetId = mqMessage.MQMD.CodedCharacterSetId,
                    Format = mqMessage.MQMD.Format,
                    Priority = mqMessage.MQMD.Priority,
                    Persistence = mqMessage.MQMD.Persistence,
                    ReplyToQueue = mqMessage.MQMD.ReplyToQueue,
                    ReplyToQueueMgr = mqMessage.MQMD.ReplyToQueueMgr,
                    UserID = mqMessage.MQMD.UserID,
                    AccountingToken = mqMessage.MQMD.AccountingToken,
                    ApplIdentityData = mqMessage.MQMD.ApplIdentityData,
                    PutApplType = mqMessage.MQMD.PutApplType,
                    PutApplName = mqMessage.MQMD.PutApplName,
                    PutDate = mqMessage.MQMD.PutDate,
                    PutTime = mqMessage.MQMD.PutTime,
                    ApplOriginData = mqMessage.MQMD.ApplOriginData,
                    GroupID = mqMessage.MQMD.GroupID,
                    MsgSequenceNumber = mqMessage.MQMD.MsgSequenceNumber,
                    Offset = mqMessage.MQMD.Offset,
                    MsgFlags = mqMessage.MQMD.MsgFlags,
                    OriginalLength = mqMessage.MQMD.OriginalLength,
                },

                // RFH2 headers is null if not found or not even tried to extract
                RFH2Headers = !rfh2Result.RFH2HeadersFound ? null : rfh2Result.Headers,

                // Only Message or MessageBytes is set, field depends whether Messages was requested as bytes or as text
                Message = !asBytes ? mqMessage.ReadString(mqMessage.MessageLength) : null,
                MessageBytes = asBytes ? mqMessage.ReadBytes(mqMessage.MessageLength) : null
            };
        }
    }
}