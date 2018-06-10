using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Saturnin.Models;
using Newtonsoft.Json;

namespace Saturnin.Helpers
{
    public class SubscriptionStore<T> where T : StoreObject
    {
        protected readonly string _storeLocation;

        public SubscriptionStore(string storeLocation)
        {
            _storeLocation = storeLocation;
            
        }

        public List<T> GetAll(string sender = "")
        {
            var json = File.ReadAllText(_storeLocation);
            var fileStore = JsonConvert.DeserializeObject<List<T>>(json);

            if (sender != "")
            {
                if(fileStore != null)
                {
                    return fileStore.Where(x => x.sender == sender).ToList();
                }
                else
                {
                    return new List<T>();
                }
            }

            if(fileStore != null)
            {
                return fileStore;
            }
            return new List<T>();
        }

        public void Add(T objectToAdd)
        {
            var fileStore = GetAll();
            fileStore.Add(objectToAdd);
            WriteToStore(fileStore);
        }

        public virtual bool Remove(T objectToRemove) {
            var fileStore = GetAll();
            var result = fileStore.RemoveAll(x => x.sender == objectToRemove.sender &&
                x.groupId == objectToRemove.groupId);
            WriteToStore(fileStore);
            return Convert.ToBoolean(result);
        }

        public void Update(T oldObject, T newObject)
        {
            Remove(oldObject);
            Add(newObject);
        }

        public int RemoveAll(string sender)
        {
            var fileStore = GetAll();
            var removedCount = fileStore.RemoveAll(x => x.sender == sender);
            WriteToStore(fileStore);

            return removedCount;
        }

        protected void WriteToStore(List<T> fileStore)
        {
            var writeToStore = JsonConvert.SerializeObject(fileStore);
            File.WriteAllText(_storeLocation, writeToStore);
        }
    }

    public class ScheduledMessagesSubscriptionStore<T> : SubscriptionStore<T> where T : ScheduledMessage
    {
        public ScheduledMessagesSubscriptionStore(string storeLocation) : base(storeLocation)
        {
        }

        public override bool Remove(T objectToRemove)
        {
            var fileStore = GetAll();
            var result = fileStore.RemoveAll(x => x.sender == objectToRemove.sender &&
                x.recipient == objectToRemove.recipient &&
                x.messageText == objectToRemove.messageText &&
                x.groupId == objectToRemove.groupId);
            WriteToStore(fileStore);
            return Convert.ToBoolean(result);
        }
    }

    public class DpmbSubscriptionStore<T> : SubscriptionStore<T> where T : DpmbSubscriber
    {
        public DpmbSubscriptionStore(string storeLocation) : base(storeLocation)
        {
        }

        public override bool Remove(T objectToRemove)
        {
            var result = RemoveAll(objectToRemove.sender, objectToRemove.lineNumber);
            return Convert.ToBoolean(result);
        }

        public int RemoveAll(string sender, string line)
        {
            var fileStore = GetAll();
            var removedCount = fileStore.RemoveAll(x => x.sender == sender && x.lineNumber == line);
            WriteToStore(fileStore);

            return removedCount;
        }
    }
}
