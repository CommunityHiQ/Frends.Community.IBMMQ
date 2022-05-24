using System;
using System.Threading;
using Frends.Community.IBMMQ.Helpers;
using IBM.WMQ;
using System.ComponentModel;

#pragma warning disable 1591

namespace Frends.Community.IBMMQ
{
    public static class PeekMessageTask
    {
        /// <summary>
        ///     Peek first message from IBM MQ queue without removing it from the queue. See https://github.com/CommunityHiQ/Frends.Community.IBMMQ
        /// </summary>
        /// <param name="input"></param>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { bool MessageFound, QueueMessage Data } where Data contains a QueueMessage if any and MessageFound is true when a message was found, otherwise false</returns>
        public static PeekMessageOutput PeekMessage(
            [PropertyTab] PeekMessageTaskParameters input,
            [PropertyTab] ConnectionParameters connection,
            [PropertyTab] TaskOptions options,
            CancellationToken cancellationToken)
        {
            // Options are different for Peek where we don't want to remove messages from queues
            int openOptions = MQC.MQOO_BROWSE | MQC.MQOO_FAIL_IF_QUIESCING | MQC.MQOO_INQUIRE;
            var getOptions = new MQGetMessageOptions
            {
                Options = MQC.MQGMO_BROWSE_FIRST
            };

            var output = new PeekMessageOutput()
            {
                MessageFound = false
            };

            using (var queueManager = IBMMQHelpers.CreateQMgrConnection(connection, options))
            {
                var queue = queueManager.AccessQueue(input.Queue, openOptions);

                var message = new MQMessage();

                try
                {
                    message = new MQMessage();
                    queue.Get(message, getOptions);
                    output.Data = IBMMQHelpers.IBMMQMessageToQueueMessage(message, input.MessageAsBytes, input.GetMessageProperties, input.GetMessageDescriptor, input.ParseRFH2Header);
                    output.MessageFound = true;
                }
                catch (MQException mqe)
                {
                    if (mqe.ReasonCode == 2033)
                    {
                        // If no messages found return with empty output
                        output.Data = null;
                    }
                    else
                    {
                        throw new Exception($"MQException received. Details: {mqe.ReasonCode} - {mqe.Message}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Exception received. Details: {ex.Message}");
                }

                queue.Close();
            }

            return output;
        }
    }
}