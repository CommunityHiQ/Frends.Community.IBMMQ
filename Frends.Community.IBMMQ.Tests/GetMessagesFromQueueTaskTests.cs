using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.IBMMQ.Tests
{
    /// <summary>
    /// You will need access to IBM MQ queue
    /// 
    /// </summary>
    //[TestFixture]
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
    }
}