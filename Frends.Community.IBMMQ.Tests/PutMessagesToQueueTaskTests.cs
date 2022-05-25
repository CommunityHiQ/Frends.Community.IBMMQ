using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.IBMMQ.Tests
{
    [Ignore("IBMMQ is not installed on build server.")]
    [TestClass]
    public class PutMessagesToQueueTaskTests
    {
        [TestMethod]
        public void PutMessagesTaskTest()
        {
            var output = PutMessageTask.PutMessage(
                new PutMessageInputParameters
                {
                    MessageContent = "hello, world (task)",
                    Queue = "DEV.QUEUE.1",
                    MessageAsBytes = false
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
                new PutMessageProperties
                {
                    CharacterSet = CharacterSetEnum.UTF8,
                    Descriptors = Array.Empty<MessageDescriptorProperty>(),
                    Properties = Array.Empty<MessageProperty>(),
                    SetRFH2Headers = true,
                    RFH2Headers = new RFH2HeaderProperty[1] { new RFH2HeaderProperty { Name = "NameValueData", Value = "TestDataÄÄ" } }
                },
                new CancellationToken());

            Assert.AreEqual(true, output.Success);
        }
    }
}