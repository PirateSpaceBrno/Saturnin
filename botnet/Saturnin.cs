using NDesk.DBus;
using Newtonsoft.Json;
using Saturnin.Helpers;
using Saturnin.Interfaces;
using Saturnin.Texts;
using Saturnin.Models.GraphApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace Saturnin
{
    public class Saturnin
    {
        #region Global variables
        private ISignal _service;
        private Thread _receiver = null;
        private ObjectPath opath = new ObjectPath(Configuration.DBUS_OBJECT_PATH);
        private Bus _conn;
        private List<KeyValuePair<byte[], string>> groupsNames = new List<KeyValuePair<byte[], string>>();
        private WebClient webClient = new WebClient();
        #endregion

        #region Constructor
        public Saturnin()
        {
            // Just for tests
        }
        public Saturnin(Bus dbus)
        {
            _conn = dbus;
            _service = _conn.GetObject<ISignal>(Configuration.DBUS_NAME, opath);
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
                _service.MessageReceived += ReactOnMessage;

                while (true)
                {
                    _conn.Iterate();
                    Thread.Sleep(1); //neccessary for correct loop work
                }
                    
            }
            catch(Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}");
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
                        case var m when (m.Equals($"{Configuration.SALUTATION}, pozdrav!", StringComparison.InvariantCultureIgnoreCase)):
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
                                SendMessage(SaturninResponses.Wait, sender, groupId);

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
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}");
            }
        }
#endregion

#region Operate Signal service
        public async virtual void SendMessage(string message, string recipient, byte[] groupId)
        {
            try
            {
                await Task.Run(() => {
                    if (groupId != null && groupId.Length > 0)
                    {
                        _service.sendGroupMessage(message, new string[] { }, groupId);
                    }
                    else
                    {
                        _service.sendMessage(message, new string[] { }, recipient);
                    }
                });

            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}");
            }
        }

        public async Task<string> GetGroupName(byte[] groupId)
        {
            try
            {
                var group = groupsNames.FirstOrDefault(x => x.Key.Equals(groupId));
                if (group.Equals(new KeyValuePair<byte[], string>()))
                {
                    string groupName = await Task.Run(() =>
                    {
                        return _service.getGroupName(groupId);
                    });

                    groupsNames.Add(new KeyValuePair<byte[], string>(groupId, groupName));

                    return groupName;
                }

                return group.Value;
            }
            catch (Exception e)
            {
                Log.Write($"{e.Message} | StackTrace: {e.StackTrace}");
                return "";
            }
        }

#endregion
    }
}