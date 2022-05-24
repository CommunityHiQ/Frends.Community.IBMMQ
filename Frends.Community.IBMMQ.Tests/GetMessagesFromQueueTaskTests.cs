using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.IBMMQ.Tests
{
    /// <summary>
    /// You will need access to IBM MQ queue. 
    /// 
    /// MQ Server can be downloaded from IBM Developer site. You may need to register as developer.
    /// 
    /// Docker image is also available: https://hub.docker.com/r/ibmcom/mq/ (though depcrecated after version 9.2.4)
    /// 
    /// </summary>
    // [TestFixture]
    // [Ignore("IBMMQ is not installed on build server.")]
    [TestClass]
    public class GetMessagesFromQueueTaskTests
    {
        [TestMethod]
        public void GetMessagesFromQueueTaskTest()
        {
            var output = GetMessagesTask.GetMessages(
                new GetMessagesTaskParameters
                {
                    MessageCount = 1,
                    Queue = "DEV.QUEUE.1",
                    MessageAsBytes = false,
                    UseTransaction = false
                },
                new ConnectionParameters
                {
                    HostName = "localhost",
                    Channel = "DEV.APP.SVRCONN",
                    QueueManagerName = "QM1",
                    PortNumber = 1414,
                    UserId = "app",
                    Password = "Passw0rdPassw0rd"
                },
                new TaskOptions
                {
                    
                },
                new CancellationToken());
            
            Assert.AreEqual(1, output.Data.Count);
        }

        [TestMethod]
        public void GetMessagesFromQueueTaskTestAsBytesMultipleMessages()
        {
            var output = GetMessagesTask.GetMessages(
                new GetMessagesTaskParameters
                {
                    MessageCount = 3,
                    Queue = "DEV.QUEUE.1",
                    MessageAsBytes = true,
                    UseTransaction = false
                },
                new ConnectionParameters
                {
                    HostName = "localhost",
                    Channel = "DEV.APP.SVRCONN",
                    QueueManagerName = "QM1",
                    PortNumber = 1414,
                    UserId = "app",
                    Password = "Passw0rdPassw0rd"
                },
                new TaskOptions
                {

                },
                new CancellationToken());

            Assert.AreEqual(3, output.Data.Count);
        }

        [TestMethod]
        public void GetMessagesFromQueueTaskTestWithTransaction()
        {
            var output = GetMessagesTask.GetMessages(
                new GetMessagesTaskParameters
                {
                    MessageCount = 3,
                    Queue = "DEV.QUEUE.1",
                    MessageAsBytes = false,
                    GetMessageDescriptor = true,
                    GetMessageProperties = true,
                    ParseRFH2Header = true,
                    UseTransaction = true,
                    TransactionType = TransactionType.RequiresNew,
                    TransactionTimeout = 10
                },
                new ConnectionParameters
                {
                    HostName = "localhost",
                    Channel = "DEV.APP.SVRCONN",
                    QueueManagerName = "QM1",
                    PortNumber = 1414,
                    UserId = "app",
                    Password = "Passw0rdPassw0rd"
                },
                new TaskOptions
                {

                },
                new CancellationToken());

            Assert.AreEqual(3, output.Data.Count);
        }
    }
}