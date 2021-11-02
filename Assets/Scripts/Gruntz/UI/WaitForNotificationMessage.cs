using System.Collections.Generic;
using System.Linq;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.TriggerBox;

namespace Gruntz.UI
{
    public class WaitForNotificationMessage : CoroutineProcess
    {
        public MessagesBoxTagDef NotificationMessagesBox;
        public ProcessContextTagDef NotificationTagDef;
        protected override IEnumerator<object> Crt()
        {
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            while (!messagesSystem.GetMessages(NotificationMessagesBox).Any()) {
                yield return null;
            }

            var notification = messagesSystem.GetMessages(NotificationMessagesBox).First().Data as TriggerShowNotificationActionDef.Notification;
            context.PutItem(NotificationTagDef, notification);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
