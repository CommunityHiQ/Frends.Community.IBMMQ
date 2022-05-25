using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.IBMMQ.Tests
{
    /// <summary>
    /// You will need access to IBM MQ queue to run these tests.
    /// 
    /// MQ Server can be downloaded from IBM Developer site (packege is about 900MB)
    /// Windows instructions: https://developer.ibm.com/tutorials/mq-connect-app-queue-manager-windows/#
    /// 
    /// If you follow the instructions at least most required steps will be done for these tests to work
    /// You may have to change Queue / Username / Password in the tests
    /// For Get and Peek tests to pass, a few messages are needed in the queue (at least 7)
    /// 
    /// </summary>
    [Ignore("IBMMQ is not installed on build server.")]
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
                    TransactionType = TransactionType.Required,
                    TransactionTimeout = 5
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