using HtmlAgilityPack;
using NDesk.DBus;
using Newtonsoft.Json;
using Saturnin.Helpers;
using Saturnin.Interfaces;
using Saturnin.Models;
using Saturnin.Models.GraphApi;
using Saturnin.Texts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Saturnin.Helpers.GeoLocationHelper;

namespace Saturnin
{
    public class Saturnin
    {
#region Global variables
        private ISignal _service;
        private Thread _receiver = null;
        private ObjectPath opath = new ObjectPath(Configuration.DBUS_OBJECT_PATH);
        private Bus _conn;
        private List<Tuple<byte[], string, DateTime>> groupsNames = new List<Tuple<byte[], string, DateTime>>();
        private WebClient webClient = new WebClient();
        private List<ScheduledMessage> scheduledMessages = new List<ScheduledMessage>();
        private List<DpmbSubscriber> dpmbSubscribers = new List<DpmbSubscriber>();
#endregion

#region Constructor
        public Saturnin()
        {
            // Just for tests
        }
        public Saturnin(Bus dbus)
        {
            Log.Write($"Connecting Saturnin to Signal service via DBus");
            Log.Line();

            try
            {
                _conn = dbus;
                _service = _conn.GetObject<ISignal>(Configuration.DBUS_NAME, opath);
            }
            catch(Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}", Log.LogLevel.FATAL);
                Environment.Exit(0);
            }
        }
#endregion

#region Read messages and react
        /// <summary>
        /// Will listen for new messages and react on them
        /// </summary>
        public void ReadMessages()
        {
            if (_receiver == null)
            {
                _receiver = new Thread(new ThreadStart(Listen));
                _receiver.Start();
            }
            else
            {
                Log.Write("Receiver already running", Log.LogLevel.WARN);
            }
        }

        /// <summary>
        /// Iterate dbus connection and read messages
        /// </summary>
        private void Listen()
        {
            try
            {
                Log.Write("Listening for messages");
                _service.MessageReceived += ReactOnMessage;

                while (true)
                {
                    _conn.Iterate();
                    Thread.Sleep(1); //neccessary for correct loop work
                }
                    
            }
            catch(Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}", Log.LogLevel.ERROR);
            }
        }

        protected async void ReactOnMessage(long timestamp, string sender, byte[] groupId, string message, string[] attachments)
        {
            try
            {
                if (message != "" && sender != "")
                {
                    if (groupId != null && groupId.Length > 0)
                    {
                        var groupName = await GetGroupName(groupId);
                        //var groupName = "{" + string.Join(",", groupId) + "}";
                        Log.Write($"Sender: {sender} in group '{groupName}' | Message: {message}");
                    }
                    else
                    {
                        Log.Write($"Sender: {sender} | Message: {message}");
                    }

                    switch (message)
                    {
                        // "get help" command
                        case var m when (m.Equals($"{Configuration.SALUTATION}?", StringComparison.InvariantCultureIgnoreCase)):
                            SendMessage(SaturninResponses.Help, sender, groupId);
                            break;
                        // "say hello" command
                        case var m when (m.StartsWith($"{Configuration.SALUTATION}, pozdrav", StringComparison.InvariantCultureIgnoreCase)):
                            SendMessage(SaturninResponses.Hello, sender, groupId);
                            break;
                        // when url present, return webpage title
                        case var m when (m.Contains("http://") || m.Contains("https://")):
                            var messageSplitted = m.Split(' ');
                            var url = messageSplitted.FirstOrDefault (x => x.StartsWith("http") || x.StartsWith("https"));
                            var urlTitle = WebHelper.GetUrlTitle(url);
                            
                            if(urlTitle != null && urlTitle != "")
                            {
                                SendMessage($"{urlTitle}\n({url})", sender, groupId);
                            }

                            break;
                        // GraphApi graph.pirati.cz - read how many members are inside specified group
                        case var m when (
                            m.StartsWith(
                                ($"{Configuration.SALUTATION}, kolik členů má "), StringComparison.InvariantCultureIgnoreCase)
                            || m.StartsWith(
                                ($"{Configuration.SALUTATION}, kolik členů má ").RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase)
                            ):
                            
                            var command = ($"{Configuration.SALUTATION}, kolik členů má ");
                            var graphApiGroupName = m.Substring(command.Length).Replace("?", "");

                            if(graphApiGroupName != "")
                            {
                                // Get groups from graph.pirati.cz
                                string graphApiGroupsJson = await webClient.DownloadStringTaskAsync("https://graph.pirati.cz/groups");

                                var graphApiGroups = JsonConvert.DeserializeObject<List<GraphApiGroup>>(graphApiGroupsJson);

                                var graphApiGroup = graphApiGroups.FirstOrDefault(g => g.username.Equals(graphApiGroupName.RemoveDiacritics(), StringComparison.InvariantCultureIgnoreCase));
                                if (graphApiGroup != null)
                                {
                                    // Get group members
                                    var graphApiGroupId = graphApiGroup.id;
                                    string graphApiMembersJson = await webClient.DownloadStringTaskAsync($"https://graph.pirati.cz/{graphApiGroupId}/members");
                                    var graphApiMembers = JsonConvert.DeserializeObject<List<GraphApiMember>>(graphApiMembersJson);
                                    var graphApiMembersCount = graphApiMembers.Count;

                                    SendMessage($"Skupina '{graphApiGroupName}' má k tomuto okamžiku {graphApiMembersCount.ToString()} členů.", sender, groupId);
                                }
                                else
                                {
                                    SendMessage($"Skupina '{graphApiGroupName}' nebyla nalezena.", sender, groupId);
                                }
                            }

                            break;
                        // GraphApi graph.pirati.cz - return list of all groups
                        case var m when m.RemoveDiacritics().StartsWith($"{Configuration.SALUTATION}, vypis vsechny skupiny", StringComparison.InvariantCultureIgnoreCase):
                            // Get groups from graph.pirati.cz
                            string graphApiGroupsListJson = await webClient.DownloadStringTaskAsync("https://graph.pirati.cz/groups");

                            var graphApiListGroups = JsonConvert.DeserializeObject<List<GraphApiGroup>>(graphApiGroupsListJson);

                            var q = string.Join(",\n", graphApiListGroups.Select(x => x.username));

                            SendMessage($"Pirátské fórum má nyní tyto skupiny:\n\n{q}", sender, groupId);

                            break;
                        // Grab random joke from lamer.cz
                        case var m when m.RemoveDiacritics().StartsWith($"{Configuration.SALUTATION}, rekni vtip", StringComparison.InvariantCultureIgnoreCase):

                            SendMessage(SaturninResponses.Wait, sender, groupId);

                            var lamerUrl = "http://www.lamer.cz/quote/random";
                            string lamerPage = await webClient.DownloadStringTaskAsync(lamerUrl);

                            HtmlDocument doc = new HtmlDocument();
                            doc.Load(lamerPage.ToStream());

                            // grab first random quote
                            var firstQuote = doc.DocumentNode.SelectNodes("//div[contains(@class, 'quote') and contains(@class ,'first')]//p[contains(@class, 'text')]")[0].InnerText;

                            // replace xml safe symbols
                            firstQuote = WebUtility.HtmlDecode(firstQuote);

                            SendMessage($"{firstQuote}\nZdroj:http://lamer.cz/", sender, groupId);

                            break;
                        // Schedule to send message
                        case var m when Regex.IsMatch(m.RemoveDiacritics(), SaturninResponses.ScheduleMessage):
                            ScheduleMessage(Regex.Match(m.RemoveDiacritics(), SaturninResponses.ScheduleMessage), sender, null);
                            break;
                        // Schedule to send message back to sender
                        case var m when Regex.IsMatch(m.RemoveDiacritics(), SaturninResponses.ScheduleMessageForMe):
                            ScheduleMessage(Regex.Match(m.RemoveDiacritics(), SaturninResponses.ScheduleMessageForMe), sender, null, true);
                            break;
                        // Unschedule messages
                        case var m when m.RemoveDiacritics().StartsWith($"{Configuration.SALUTATION}, zrus me naplanovane zpravy"):
                            var removedMessagesCount = scheduledMessages.RemoveAll(x => x.sender == sender);

                            SendMessage($"Odebral jsem všech tvých {removedMessagesCount} naplánovaných zpráv.", sender, null);
                            break;
                        // How many scheduled messages I have
                        case var m when m.RemoveDiacritics().StartsWith($"{Configuration.SALUTATION}, kolik mam naplanovanych zprav?"):
                            int messageCount = scheduledMessages.Where(x => x.sender == sender).Count();
                            if(messageCount == 0)
                            {
                                SendMessage("Nemáš žádné naplánované zprávy.", sender, null);
                            }
                            else
                            {
                                SendMessage($"Právě máš naplánováno {messageCount} zpráv.", sender, null);
                            }
                            break;
                        // DPMB
                        case var m when Regex.IsMatch(m.RemoveDiacritics(), $@"{Configuration.SALUTATION}, sleduj DPMB linku (\d+) v okruhu (\d+) metru od bodu (.*),(.*)", RegexOptions.IgnoreCase):
                            SubscribeOnDpmb(Regex.Match(m.RemoveDiacritics(), $@"{Configuration.SALUTATION}, sleduj DPMB linku (\d+) v okruhu (\d+) metru od bodu (.*),(.*)", RegexOptions.IgnoreCase), sender, groupId);
                            break;
                        case var m when Regex.IsMatch(m.RemoveDiacritics(), $@"{Configuration.SALUTATION}, prestan sledovat DPMB linku (\d+)", RegexOptions.IgnoreCase):
                            Match match1 = Regex.Match(m.RemoveDiacritics(), $@"{Configuration.SALUTATION}, prestan sledovat DPMB linku (\d+)", RegexOptions.IgnoreCase);
                            var line = match1.Groups[1].Value.ToString();
                            RemoveDpmbSubscribers(sender, groupId, line);
                            break;
                        case var m when Regex.IsMatch(m.RemoveDiacritics(), $@"{Configuration.SALUTATION}, prestan sledovat vsechny DPMB linky", RegexOptions.IgnoreCase):
                            RemoveDpmbSubscribers(sender, groupId);
                            break;
                        case var m when Regex.IsMatch(m.RemoveDiacritics(), $@"{Configuration.SALUTATION}, jake sledujes DPMB linky", RegexOptions.IgnoreCase):
                            ListAllMyDpmbSubscribers(sender, groupId);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}", Log.LogLevel.ERROR);
            }
        }
#endregion

#region Operate Signal service
        public async virtual void SendMessage(string message, string recipient, byte[] groupId)
        {
            try
            {
                await Task.Run(async () => {
                    if (groupId != null && groupId.Length > 0)
                    {
                        string groupName = await GetGroupName(groupId);
                        Log.Write($"Sending group message with text '{message}' to group '{groupName}'");
                        _service.sendGroupMessage(message, new string[] { }, groupId);
                    }
                    else
                    {
                        Log.Write($"Sending message with text '{message}' to '{recipient}'");
                        _service.sendMessage(message, new string[] { }, recipient);
                    }
                });

            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}", Log.LogLevel.ERROR);
            }
        }

        /// <summary>
        /// Ask Signal for group name according to its ID and store it for further use.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<string> GetGroupName(byte[] groupId)
        {
            try
            {
                var group = groupsNames.FirstOrDefault(x => x.Item1.SequenceEqual(groupId));
                if (group == null || DateTime.Now.Subtract(group.Item3).TotalDays >= 1)
                {
                    Log.Write($"Fetching group name for {string.Join(",", groupId)}");
                    Log.Write($"Groups names in memory: {groupsNames.Count.ToString()}", Log.LogLevel.DEBUG);

                    return await Task.Run(() =>
                    {
                        string groupName = _service.getGroupName(groupId);

                        if(group != null)
                        {
                            groupsNames.Remove(group);
                        }
                        
                        groupsNames.Add(new Tuple<byte[], string, DateTime>(groupId, groupName, DateTime.Now));

                        Log.Write($"Group members: {string.Join(", ", _service.getGroupMembers(groupId))}", Log.LogLevel.DEBUG);

                        return groupName;
                    });
                }
                else
                {
                    Log.Write($"Loaded name {group.Item2} for {string.Join(",", groupId)}", Log.LogLevel.DEBUG);
                    return group.Item2;
                }
            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}", Log.LogLevel.ERROR);
                return "";
            }
        }

        /// <summary>
        /// Returns all connected Signal groups IDs.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<byte[]>> GetGroupIds()
        {
            return await Task.Run(() =>
            {
                return _service.getGroupIds();
            });
        }

#endregion

#region Scheduled Messages
        private struct ScheduledMessage
        {
            public DateTime scheduledTime;
            public string recipient;
            public string messageText;
            public string sender;
            public byte[] groupId;
        }

        public void WatchScheduledMessages()
        {
            System.Timers.Timer _scheduledSender = new System.Timers.Timer(Configuration.POLLINGINTERVAL);
            _scheduledSender.Elapsed += PollScheduledMessages;
            _scheduledSender.Start();
        }

        private void PollScheduledMessages(object sender, EventArgs e)
        {
            var storeCount = scheduledMessages.Count;

            Log.Write($"Polling for scheduled messages, actual count in store = {storeCount}", Log.LogLevel.DEBUG);

            if(storeCount > 0)
            {
                foreach(var scheduledMessage in scheduledMessages)
                {
                    if(scheduledMessage.scheduledTime <= DateTime.Now)
                    {
                        Log.Write($"Processing message scheduled on {scheduledMessage.scheduledTime.ToString()}");


                        if(scheduledMessage.sender != scheduledMessage.recipient)
                        {
                            var messageContent = $"\"{scheduledMessage.messageText}\"\n" +
    $"\n" +
    $"Tuto zprávu Ti nechal zaslat váš známý s číslem {scheduledMessage.sender}.";

                            SendMessage(messageContent, scheduledMessage.recipient, null);
                            SendMessage($"Právě jsem odeslal tvou naplánovanou zprávu pro {scheduledMessage.recipient}.", scheduledMessage.sender, null);
                        }
                        else
                        {
                            SendMessage(scheduledMessage.messageText, scheduledMessage.recipient, scheduledMessage.groupId);
                        }
                            

                        scheduledMessages.Remove(scheduledMessage);
                    }
                }
            }
        }

        public void ScheduleMessage(Match match, string sender, byte[] groupId, bool senderIsRecipient = false)
        {
            var scheduledMessageTime = DateTime.Parse(match.Groups[4].Value);
            var scheduledMessageText = match.Groups[7].Value;
            var scheduledMessageRecipient = senderIsRecipient ? sender : match.Groups[9].Value;

            // If time is already past, add one day
            if (scheduledMessageTime < DateTime.Now)
            {
                scheduledMessageTime = scheduledMessageTime.AddDays(1);
            }

            var scheduledMessage = new ScheduledMessage()
            {
                messageText = scheduledMessageText,
                recipient = scheduledMessageRecipient,
                scheduledTime = scheduledMessageTime,
                sender = sender,
                groupId = groupId
            };

            scheduledMessages.Add(scheduledMessage);

            if(senderIsRecipient)
                SendMessage($"Naplánoval jsem odeslání zprávy na tvé číslo dne {scheduledMessageTime.ToString("dd.MM.yyyy v HH:mm")}.", sender, groupId);
            else
                SendMessage($"Naplánoval jsem odeslání zprávy na číslo '{scheduledMessageRecipient}' dne {scheduledMessageTime.ToString("dd.MM.yyyy v HH:mm")}.", sender, null);
        }
#endregion

#region DPMB
        public void SubscribeOnDpmb(Match match, string sender, byte[] groupId)
        {
            var dpmbSubscriber = new DpmbSubscriber()
            {
                lineNumber = match.Groups[1].Value.ToString(),
                radius = Convert.ToDouble(match.Groups[2].Value),
                centerPoint = new GeoCoordinations()
                {
                    latitude = Convert.ToDouble(match.Groups[3].Value),
                    longitude = Convert.ToDouble(match.Groups[4].Value)
                },
                sender = sender,
                groupId = groupId,
                lastSentMessage = DateTime.Now
            };

            // Remove previous subscriber of this line
            dpmbSubscribers.Add(dpmbSubscriber);

            SendMessage($"DPMB linku {dpmbSubscriber.lineNumber} jsem přidal do sledovaných.", sender, groupId);
        }

        public struct DpmbSubscriber
        {
            public string lineNumber;
            public GeoCoordinations centerPoint;
            public double radius;
            public string sender;
            public byte[] groupId;
            public DateTime lastSentMessage;
        }

        private void PollDpmbSubscribers(object sender, EventArgs e)
        {
            var storeCount = dpmbSubscribers.Count;

            Log.Write($"Polling for DPMB subscribers, actual count in store = {storeCount}", Log.LogLevel.DEBUG);

            if (storeCount > 0)
            {
                foreach (var subscriber in dpmbSubscribers)
                {
                    if(DateTime.Now.Subtract(subscriber.lastSentMessage).TotalMinutes > 1)
                    {
                        var buses = WatchDpmbLine(subscriber).Result;
                        if (buses.Count > 0)
                        {
                            var message =
                                $"V okruhu {subscriber.radius.ToString()} metrů od bodu [{subscriber.centerPoint.latitude.ToString()},{subscriber.centerPoint.longitude.ToString()}] se právě nachází {buses.Count.ToString()} vozů DPMB linky {subscriber.lineNumber}.\n";

                            foreach (var bus in buses)
                            {
                                if (bus.headsign != "")
                                    message += $"\n{bus.headsign}";
                            }

                            // Change lastSentTime in subscriber
                            var newSubscriber = subscriber;
                            newSubscriber.lastSentMessage = DateTime.Now;
                            dpmbSubscribers.Remove(subscriber);
                            dpmbSubscribers.Add(newSubscriber);

                            SendMessage(message, subscriber.sender, subscriber.groupId);
                        }
                    }
                }
            }
        }

        public void WatchDpmbSubscribers()
        {
            System.Timers.Timer _scheduledSender = new System.Timers.Timer(Configuration.DPMBPOLLINGINTERVAL);
            _scheduledSender.Elapsed += PollDpmbSubscribers;
            _scheduledSender.Start();
        }

        public void RemoveDpmbSubscribers(string sender, byte[] groupId, string line = "")
        {
            if(line != "")
            {
                dpmbSubscribers.RemoveAll(x => x.sender == sender && x.lineNumber == line);
                SendMessage($"Odebral jsem všechna tvá sledování DPMB linky {line}.", sender, groupId);
            }
            else
            {
                dpmbSubscribers.RemoveAll(x => x.sender == sender);
                SendMessage($"Odebral jsem všechna tvá sledování DPMB linek.", sender, groupId);
            }
        }

        public void ListAllMyDpmbSubscribers(string sender, byte[] groupId)
        {
            var mySubscribers = dpmbSubscribers.Where(x => x.sender == sender).Select(x => new KeyValuePair<string, GeoCoordinations>(x.lineNumber, x.centerPoint));

            var lines = string.Join("\n", mySubscribers.Select(x => $"{x.Key} [{x.Value.latitude.ToString()},{x.Value.longitude.ToString()}]"));

            SendMessage(
                $"Sleduji tyto DPMB linky:\n{lines}",
                sender, groupId);
        }
        #endregion
    }
}