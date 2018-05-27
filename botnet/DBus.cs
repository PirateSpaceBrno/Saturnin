using System;
using System.Collections.Generic;
using System.Threading;
using DBus;
using org.freedesktop.DBus;

namespace botnet
{
    public static class DBus
    {
        public static void Test()
        {
            Bus bus = Bus.System;
            var g = bus.IsConnected;

            var sample = bus.GetObject<ISignal>("org.asamk.Signal", new ObjectPath("/org/asamk/Signal"));

            try
            {
                sample.sendMessage("Testovací zpráva", new List<string>(), "+420608828650");
            }
            catch(Exception e)
            {
                Thread.Sleep(60 * 1000);
            }
        }

        [Interface("org.asamk.Signal")]
        public interface ISignal
        {
            void sendMessage(string message, List<string> attachments, string recipient);
            void sendMessage(String message, List<String> attachments, List<String> recipients);
            void sendEndSessionMessage(List<String> recipients);
            void sendGroupMessage(string message, List<string> attachments, byte[] groupId);
            String getContactName(String number);
            void setContactName(String number, String name);
            List<byte[]> getGroupIds();
            List<string> getGroupMembers(byte[] groupId);
            byte[] updateGroup(byte[] groupId, string name, List<string> members, string avatar);
        }
    }
}