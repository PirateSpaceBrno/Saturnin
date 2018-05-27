using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace botnet
{
    public static class SignalCliCommands
    {
        private static readonly string SignalCli = $"signal-cli -u {Configuration.USERNAME}";


        /*
        byte[] bytes = Encoding.ASCII.GetBytes(someString);
        dbus-send --system --type=method_call --print-reply --dest='org.asamk.Signal' /org/asamk/Signal org.asamk.Signal.sendMessage string:'MessageText' array:string: string:'+420608828650'
        
        */


        public static async Task<string> ListGroups()
        {
            string groups = await ExecuteBashCommand($"{SignalCli} listGroups");
            return groups;
        }

        public static async void SendMessage(string message, string receiver, bool isGroup = false)
        {
            if(isGroup)
            {
                receiver = $"-g {receiver}";
            }

            Log.Write($"{SignalCli} send -m '{message}' {receiver}", Log.LogLevel.DEBUG);
            await ExecuteBashCommand($"{SignalCli} send -m '{message}' {receiver}");
        }

        internal static async Task<string> ListenForMessagesAsync()
        {
            return await ExecuteBashCommand($"{SignalCli} receive -t {Configuration.TIMEOUT.ToString()} --json");
        }

        public static async void Receive()
        {
            Log.Write("Receiving bulk of messages");

            var messages = await ListenForMessagesAsync();
            Log.Write(messages, Log.LogLevel.DEBUG);

            if(messages == "") { return; }

            Log.Write("Processing received messages");

            var messagesObjects = messages.Split('\n');
            foreach (string messageString in messagesObjects)
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<SignalMessage>(messageString);
                    if (message == null) break;
                    if (message.envelope.dataMessage == null) break;

                    var messageContent = message.envelope.dataMessage.message;
                    var messageSender = message.envelope.source;
                    var messageRespondTo = messageSender;
                    var isGroupChat = false;

                    if (message.envelope.dataMessage.groupInfo != null)
                    {
                        messageRespondTo = message.envelope.dataMessage.groupInfo.groupId;
                        isGroupChat = true;
                    }

                    if (messageContent.StartsWith(Configuration.SALUTATION, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Log.Line();
                        Log.Write($"Sender: {messageSender}");
                        Log.Write($"Message: {messageContent}");
                        Log.Write($"Respond to: {messageRespondTo}");

                        switch (messageContent)
                        {
                            // "get help" command
                            case var m when (m.Equals($"{Configuration.SALUTATION}?", StringComparison.InvariantCultureIgnoreCase)):
                                SendMessage(SaturninResponses.Help, messageRespondTo, isGroupChat);
                                break;
                            // "say hello" command
                            case var m when (m.Equals($"{Configuration.SALUTATION} pozdrav!", StringComparison.InvariantCultureIgnoreCase)):
                                SendMessage(SaturninResponses.Hello, messageRespondTo, isGroupChat);
                                break;
                        }

                        Log.Line();
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " | " + e.StackTrace);
                }
            }
        }

        private static async Task<string> ExecuteBashCommand(string command)
        {
            // according to: https://stackoverflow.com/a/15262019/637142
            // thanks to this we will pass everything as one command
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return await proc.StandardOutput.ReadToEndAsync();
        }
    }
}
