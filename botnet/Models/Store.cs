using System;
using static Saturnin.Helpers.GeoLocationHelper;

namespace Saturnin.Models
{
    public class StoreObject
    {
        public string sender;
        public byte[] groupId;
    }

    public class ScheduledMessage : StoreObject
    {
        public DateTime scheduledTime;
        public string recipient;
        public string messageText;
    }

    public class DpmbSubscriber : StoreObject
    {
        public string lineNumber;
        public GeoCoordinations centerPoint;
        public double radius;
        public DateTime lastSentMessage;
    }

    public class GroupIdentification : StoreObject
    {
        public DateTime lastSync;
        public string groupName;
    }
}
