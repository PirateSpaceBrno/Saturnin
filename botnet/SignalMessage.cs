namespace botnet
{
    public class SignalMessage
    {
        public Envelope envelope { get; set; }
    }

    public class Envelope
    {
        public string source { get; set; }
        public int sourceDevice { get; set; }
        public object relay { get; set; }
        public long timestamp { get; set; }
        public bool isReceipt { get; set; }
        public Datamessage dataMessage { get; set; }
        public object syncMessage { get; set; }
        public object callMessage { get; set; }
    }

    public class Datamessage
    {
        public long timestamp { get; set; }
        public string message { get; set; }
        public int expiresInSeconds { get; set; }
        public object[] attachments { get; set; }
        public Groupinfo groupInfo { get; set; }
    }

    public class Groupinfo
    {
        public string groupId { get; set; }
        public object members { get; set; }
        public object name { get; set; }
        public string type { get; set; }
    }
}