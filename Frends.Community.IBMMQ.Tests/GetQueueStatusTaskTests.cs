using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.IBMMQ.Tests
{
    [Ignore("IBMMQ is not installed on build server.")]
    [TestClass]
    public class GetQueueStatusTaskTests
    {
        [TestMethod]
        public void GetQueueStatusTaskTest()
        {
            var output = GetQueueStatusTask.GetQueueStatus(
                new GetQueueStatusInput
                {
                    Queues = new string[] { "DEV.QUEUE.1" }
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
            
            Assert.AreEqual(1, output.Count);
        }
    }
}