using System.Threading;
using Frends.Community.IBMMQ.Helpers;
using IBM.WMQ;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Diagnostics;

#pragma warning disable 1591

namespace Frends.Community.IBMMQ
{
    public static class GetQueueStatusTask
    {
        /// <summary>
        ///     Get status information of queues. See https://github.com/CommunityHiQ/Frends.Community.IBMMQ
        /// </summary>
        /// <param name="input"></param>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>List&lt;GetQueueStatusOutput&gt; where GetQueueStatusOutput has the following properties: string Name, int MessageCount, string Description, DateTime OldestMessageQueueTime</returns>
        public static List<GetQueueStatusOutput> GetQueueStatus(
            [PropertyTab] GetQueueStatusInput input,
            [PropertyTab] ConnectionParameters connection,
            [PropertyTab] TaskOptions options,
            CancellationToken cancellationToken)
        {
            // Options are different here where we don't want to remove messages from queues
            int openOptions = MQC.MQOO_BROWSE | MQC.MQOO_FAIL_IF_QUIESCING | MQC.MQOO_INQUIRE;
            var getOptions = new MQGetMessageOptions
            {
                Options = MQC.MQGMO_BROWSE_FIRST
            };
            
            var resultList = new List<GetQueueStatusOutput>();

            using (var queueManager = IBMMQHelpers.CreateQMgrConnection(connection, options))
            {                

                foreach (var queueName in input.Queues)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    try {
                        var queue = queueManager.AccessQueue(queueName, openOptions);

                        var statusInfo = new GetQueueStatusOutput
                        {
                            Name = queueName,
                            MessageCount = queue.CurrentDepth,
                            Description = queue.Description.Trim().Length == 0 ? "" : queue.Description                            
                        };

                        // If queue has messages, read message from the head without removing
                        // and get the time when it was put to the queue
                        if (queue.CurrentDepth > 0)
                        {
                            var message = new MQMessage();
                            queue.Get(message, getOptions);

                            statusInfo.OldestMessageQueueTime = message.PutDateTime.ToLocalTime();
                        }
                        resultList.Add(statusInfo);

                        queue.Close();
                    }
                    catch (Exception ex)
                    {
                        var statusInfo = new GetQueueStatusOutput
                        {
                            Name = queueName,
                            MessageCount = -1,
                            Description = "Error : " + ex.Message
                        };

                        resultList.Add(statusInfo);
                    }
                }
            }

            return resultList;
        }
    }
}