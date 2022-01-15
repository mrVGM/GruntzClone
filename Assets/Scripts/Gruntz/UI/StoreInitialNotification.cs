using System.Collections.Generic;
using System.Linq;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.TriggerBox;

namespace Gruntz.UI
{
    public class StoreInitialNotification : CoroutineProcess
    {
        public MessagesBoxTagDef NotificationMessagesBox;
        public ProcessContextTagDef InitialNotificationTagDef;
        protected override IEnumerator<object> Crt()
        {
            context.PutItem(InitialNotificationTagDef, null);
            
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            var messages = messagesSystem.GetMessages(NotificationMessagesBox);
            if (!messages.Any()) {
                yield break;
            }
            var message = messagesSystem.GetMessages(NotificationMessagesBox).First();
            var notifications = message.Data as IEnumerable<NotificationDataBehaviour.Notification>;
            context.PutItem(InitialNotificationTagDef, notifications);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
