using System.Collections.Generic;
using NDesk.DBus;

namespace Saturnin.Interfaces
{
    [Interface("org.asamk.Signal")]
    public interface ISignal
    {
        void sendMessage(string message, string[] attachments, string recipient);// throws EncapsulatedExceptions, AttachmentInvalidException, IOException;
        void sendMessage(string message, string[] attachments, IList<string> recipients);// throws EncapsulatedExceptions, AttachmentInvalidException, IOException;
        void sendEndSessionMessage(string[] recipients);// throws IOException, EncapsulatedExceptions;
        void sendGroupMessage(string message, string[] attachments, byte[] groupId);// throws EncapsulatedExceptions, GroupNotFoundException, AttachmentInvalidException, IOException;
        string getContactName(string number);
        void setContactName(string number, string name);
        IList<byte[]> getGroupIds();
        string getGroupName(byte[] groupId);
        string[] getGroupMembers(byte[] groupId);
        byte[] updateGroup(byte[] groupId, string name, string[] members, string avatar);// throws IOException, EncapsulatedExceptions, GroupNotFoundException, AttachmentInvalidException;

        event MessageReceived MessageReceived;
        event ReceiptReceived ReceiptReceived;
    }


    public delegate void MessageReceived(long timestamp, string sender, byte[] groupId, string message, string[] attachments);
    public delegate void ReceiptReceived(string objectpath, long timestamp, string sender);

}







