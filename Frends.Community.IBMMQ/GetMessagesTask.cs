using System;
using System.Threading;
using Frends.Community.IBMMQ.Helpers;
using IBM.WMQ;
using System.ComponentModel;
using System.Transactions;

#pragma warning disable 1591

namespace Frends.Community.IBMMQ
{
    public static class GetMessagesTask
    {
        /// <summary>
        ///     Gets messages from IBM MQ queue. See https://github.com/CommunityHiQ/Frends.Community.IBMMQ
        /// </summary>
        /// <param name="input"></param>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Object { List&lt;QueueMessage&gt; Data }, where QueueMessages has several properties including Message or MessageBytes which contains the retrieved message</returns>
        public static GetMessagesOutput GetMessages(
            [PropertyTab] GetMessagesTaskParameters input,
            [PropertyTab] ConnectionParameters connection,
            [PropertyTab] TaskOptions options,
            CancellationToken cancellationToken)
        {
            var output = new GetMessagesOutput();

            using (var queueManager = IBMMQHelpers.CreateQMgrConnection(connection, options))
            {
                // Only use transaction if desired
                if (input.UseTransaction)
                    using (var ts = new TransactionScope(IBMMQHelpers.MapTransctionType(input.TransactionType.ToString()), TimeSpan.FromSeconds(input.TransactionTimeout)))
                    {
                        var queue = queueManager.AccessQueue(input.Queue, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);

                        var message = new MQMessage();

                        // Create GetMessageOptions object
                        // Needed when transaction is used
                        var getMessageOptions = new MQGetMessageOptions();
                        getMessageOptions.Options += MQC.MQGMO_SYNCPOINT;

                        for (var i = 1; i <= input.MessageCount; i++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            try
                            {
                                message = new MQMessage();
                                queue.Get(message, getMessageOptions);

                                output.Data.Add(IBMMQHelpers.IBMMQMessageToQueueMessage(message, input.MessageAsBytes, input.GetMessageProperties, input.GetMessageDescriptor, input.ParseRFH2Header));
                            }
                            catch (MQException mqe)
                            {
                                if (mqe.ReasonCode == 2033)
                                {
                                    // If no messages found return with empty output / messages already fetched
                                }
                                else
                                {
                                    throw new Exception($"MQException received. Details: {mqe.ReasonCode} - {mqe.Message}");
                                }

                                break;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Exception received. Details: {ex.Message}");
                            }
                        }
                        queue.Close();

                        // If using transaction perform Commit
                        ts.Complete();
                    }

                else // Transaction not used
                {
                    var queue = queueManager.AccessQueue(input.Queue, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);

                    var message = new MQMessage();

                    for (var i = 1; i <= input.MessageCount; i++)
                    {

                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            message = new MQMessage();
                            queue.Get(message);
                            output.Data.Add(IBMMQHelpers.IBMMQMessageToQueueMessage(message, input.MessageAsBytes, input.GetMessageProperties, input.GetMessageDescriptor, input.ParseRFH2Header));
                        }
                        catch (MQException mqe)
                        {
                            if (mqe.ReasonCode == 2033)
                            {
                                // If no messages found return with empty output / messages already fetched
                            }
                            else
                            {
                                throw new Exception($"MQException received. Details: {mqe.ReasonCode} - {mqe.Message}");
                            }

                            break;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Exception received. Details: {ex.Message}");
                        }
                    }
                    queue.Close();
                }
            }

            return output;
        }
    }
}