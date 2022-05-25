using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.IBMMQ.Tests
{
    [Ignore("IBMMQ is not installed on build server.")]
    [TestClass]
    public class PeekMessageFromQueueTaskTests
    {
        [TestMethod]
        public void PeekMessageFromQueueTaskTest()
        {
            var output = PeekMessageTask.PeekMessage(
                new PeekMessageTaskParameters
                {
                    Queue = "DEV.QUEUE.1",
                    MessageAsBytes = false,
                    GetMessageDescriptor = false,
                    GetMessageProperties = false,
                    ParseRFH2Header = false
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
            
            Assert.AreEqual(true, output.MessageFound);
        }
    }
}