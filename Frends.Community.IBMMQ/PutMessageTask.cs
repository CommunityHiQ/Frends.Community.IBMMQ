using System.Threading;
using Frends.Community.IBMMQ.Helpers;
using IBM.WMQ;
using System.ComponentModel;

#pragma warning disable 1591

namespace Frends.Community.IBMMQ
{
    public static class PutMessageTask
    {
        /// <summary>
        ///     Puts a message to an IBM MQ queue. See https://github.com/CommunityHiQ/Frends.Community.IBMMQ
        /// </summary>
        /// <param name="input">Message and other input parameters</param>
        /// <param name="connection">Connection parameters</param>
        /// <param name="options">SSL Connection options</param>
        /// <param name="messageProperties">Message properties (including descriptor and RFH2 headers)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>bool Success</returns>
        public static PutMessageOutput PutMessage(
            [PropertyTab] PutMessageInputParameters input,
            [PropertyTab] ConnectionParameters connection,
            [PropertyTab] TaskOptions options,
            [PropertyTab] PutMessageProperties messageProperties,
            CancellationToken cancellationToken)
        {
            using (var queueManager = IBMMQHelpers.CreateQMgrConnection(connection, options))
            {
                var queue = queueManager.AccessQueue(input.Queue, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);
                var message = new MQMessage();
                
                // Set Message properties and rfh2 headers
                IBMMQHelpers.SetMessagePropertiesAndHeaders(message, messageProperties, input.MessageAsBytes);                
                
                if(input.MessageAsBytes)
                    message.Write(input.MessageContentBytes);
                else
                    message.WriteString(input.MessageContent);

                queue.Put(message);
                queue.Close();
            }

            return new PutMessageOutput { Success = true };
        }
    }
}