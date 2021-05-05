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
            public object Sender;
            public long Time;
            public object Data;
        }

        private Dictionary<MessagesBoxTagDef, List<Message>> messages { get; } = new Dictionary<MessagesBoxTagDef, List<Message>>();

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var game = Game.Instance;
                return game.DefRepositoryDef.AllDefs.OfType<MessagesSystemExecutionOrderTagDef>().FirstOrDefault();
            }
        }

        public void DisposeObject()
        {
            Game.Instance.MainUpdater.UnRegisterUpdatable(this);
            messages.Clear();
        }

        public void DoUpdate()
        {
            messages.Clear();
        }

        public MessagesSystem()
        {
            Game.Instance.MainUpdater.RegisterUpdatable(this);
        }

        public void SendMessage(MessagesBoxTagDef messagesBoxTag, object sender, object data)
        {
            List<Message> list = null;
            if (!messages.TryGetValue(messagesBoxTag, out list))
            {
                list = new List<Message>();
                messages[messagesBoxTag] = list;
            }
            list.Add(new Message { Time = Time.frameCount, MessageBoxTag = messagesBoxTag, Sender = sender, Data = data });
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
    }
}
