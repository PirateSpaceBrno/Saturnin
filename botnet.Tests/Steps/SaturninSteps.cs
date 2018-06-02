using System.Text;
using System.Linq;
using TechTalk.SpecFlow;
using System;
using NUnit.Framework;

namespace Saturnin.Tests.Steps
{
    [Binding]
    public class SaturninSteps : Saturnin
    {

        [When(@"I receive fake message with text '(.*)' from '(.*)' in group '(.*)'")]
        public void StoreFakeReceivedMessage(string message, string sender, string groupId)
        {
            ScenarioContext.Current.Add("Received message", new SignalMessage()
            {
                message = message,
                recipient = sender,
                groupId = Encoding.Default.GetBytes(groupId)
            });
        }

  
        [Then(@"Saturnin respond with message '(.*)' to '(.*)' in group '(.*)'")]
        public void TriggerReactOnMessageMethod(string message, string recipient, string groupId)
        {
            ScenarioContext.Current.Add("Expected message", new SignalMessage()
            {
                message = message,
                recipient = recipient,
                groupId = Encoding.Default.GetBytes(groupId)
            });

            SignalMessage messageReceived = ScenarioContext.Current.Get<SignalMessage>("Received message");
            
            ReactOnMessage(0, messageReceived.recipient, messageReceived.groupId, messageReceived.message, new string[] { });
        }

        
        public override void SendMessage(string message, string recipient, byte[] groupId)
        {
            SignalMessage sentMessage = new SignalMessage()
            {
                message = message,
                recipient = recipient,
                groupId = groupId
            };

            var expectedMessage = ScenarioContext.Current.Get<SignalMessage>("Expected message");

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedMessage.recipient, recipient);
                Assert.AreEqual(expectedMessage.groupId, groupId);
                Assert.AreEqual(expectedMessage.message, message);
            });
        }


        private struct SignalMessage
        {
            public string message;
            public string recipient;
            public byte[] groupId;
        }
    }
}
