using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.MessagesSystem
{
    public class MessagesSystem : IOrderedUpdate, IContextObject
    {
        public struct Message
        {
            public MessagesBoxTagDef MessageBoxTag;
            public MainUpdaterUpdateTime TimeToClear;
            public object Sender;
            public long Time;
            public object Data;
        }

        private Dictionary<MessagesBoxTagDef, List<Message>> messages { get; } = new Dictionary<MessagesBoxTagDef, List<Message>>();
        private MessagesSystemDef _messagesSystemDef;

        public ExecutionOrderTagDef OrderTagDef => _messagesSystemDef.MessagesSystemExecutionOrderTagDef;

        public void DisposeObject()
        {
            Game.Instance.MainUpdater.UnRegisterUpdatable(this);
            messages.Clear();
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            foreach (var pair in messages) {
                var list = pair.Value;
                if (list == null) {
                    continue;
                }
                list.RemoveAll(x => x.TimeToClear == updateTime);
            }
        }

        public MessagesSystem(MessagesSystemDef messagesSystemDef)
        {
            _messagesSystemDef = messagesSystemDef;
            Game.Instance.MainUpdater.RegisterUpdatable(this);
        }

        public void SendMessage(MessagesBoxTagDef messagesBoxTag, MainUpdaterUpdateTime timeToClear, object sender, object data)
        {
            List<Message> list = null;
            if (!messages.TryGetValue(messagesBoxTag, out list))
            {
                list = new List<Message>();
                messages[messagesBoxTag] = list;
            }
            list.Add(new Message {
                Time = Time.frameCount,
                MessageBoxTag = messagesBoxTag,
                TimeToClear = timeToClear,
                Sender = sender,
                Data = data
            });
        }
        public IEnumerable<Message> GetMessages(MessagesBoxTagDef messagesBoxTag)
        {
            List<Message> list = null;
            if (!messages.TryGetValue(messagesBoxTag, out list))
            {
                return Enumerable.Empty<Message>();
            }
            return list;
        }

        public static MessagesSystem GetMessagesSystemFromContext()
        {
            var game = Game.Instance;
            var messagesSystemDef = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messagesSystemDef) as MessagesSystem;
            return messagesSystem;
        }
    }
}
