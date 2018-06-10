using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Saturnin.Helpers;
using Saturnin.Models;
using NUnit.Framework;

namespace Saturnin.Tests.Steps
{
    [Binding]
    public class SubscriptionStoreSteps
    {
        [When(@"I add message scheduled on '(.*)' from '(.*)' to '(.*)' with text '(.*)'")]
        public void ScheduleMessage(string date, string from, string to, string message)
        {
            ScheduledMessagesSubscriptionStore<ScheduledMessage> subscriptionStore = new ScheduledMessagesSubscriptionStore<ScheduledMessage>($@"D:\_projects\Saturnin\botnet\bin\Debug\ScheduledMessages.json");
            subscriptionStore.Add(new ScheduledMessage()
            {
                groupId = null,
                messageText = message,
                recipient = to,
                sender = from,
                scheduledTime = DateTime.MaxValue
            });
        }

        [Then(@"I am able to delete message scheduled  on '(.*)' from '(.*)' to '(.*)' with text '(.*)'")]
        public void RemoveMessage(string date, string from, string to, string message)
        {
            var store = new ScheduledMessagesSubscriptionStore<ScheduledMessage>($@"D:\_projects\Saturnin\botnet\bin\Debug\ScheduledMessages.json");

            var removingMessage = new ScheduledMessage()
            {
                groupId = null,
                messageText = message,
                recipient = to,
                sender = from,
                scheduledTime = DateTime.MaxValue
            };

            store.Remove(removingMessage);

            Assert.IsEmpty(store.GetAll());
        }

        [Then("I am able to delete DPMB line subscription")]
        public void RemoveDPMB()
        {
            var store = new DpmbSubscriptionStore<DpmbSubscriber>($@"D:\_projects\Saturnin\botnet\bin\Debug\DpmbSubscribers.json");

            var removingMessage = new DpmbSubscriber()
            {
                centerPoint = new GeoLocationHelper.GeoCoordinations()
                {
                    latitude = 49.19083,
                    longitude = 16.60498
                },
                groupId = "ysPERdHhCBE+LHk1KOCDPA==".ToByteArray(),
                lastSentMessage = DateTime.Parse("2018-06-10T02:35:19.97453+02:00"),
                lineNumber = "95",
                radius = 150.0,
                sender = "+420608828650"
            };

            store.Remove(removingMessage);
        }
    }
}
 