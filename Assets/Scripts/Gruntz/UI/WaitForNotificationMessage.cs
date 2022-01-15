using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.MessagesSystem;
using Base.Navigation;
using Base.UI;
using Gruntz.TriggerBox;

namespace Gruntz.UI
{
    public class WaitForNotificationMessage : CoroutineProcess
    {
        public MessagesBoxTagDef NotificationMessagesBox;
        public ProcessContextTagDef NotificationTagDef;
        public ProcessContextTagDef SelectedActorsTagDef;
        protected override IEnumerator<object> Crt()
        {
            while (true) {
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                while (!messagesSystem.GetMessages(NotificationMessagesBox).Any()) {
                    yield return null;
                }

                var selectedActors = context.GetItem(SelectedActorsTagDef) as IEnumerable<Actor>;
                if (selectedActors == null || selectedActors.Count() != 1) {
                    yield return null;
                    continue;
                }

                var selected = selectedActors.FirstOrDefault();
                var navAgent = selected.GetComponent<NavAgent>();

                var notificationMessage = messagesSystem.GetMessages(NotificationMessagesBox).First();
                var notificationActor = notificationMessage.Sender as Actor;

                if (navAgent.Target.Proximity(notificationActor.Pos) >= 0.0001f) {
                    yield return null;
                    continue;
                }

                var notification =
                    notificationMessage.Data as
                        NotificationDataBehaviour.Notification;
                
                context.PutItem(NotificationTagDef, notification);
                break;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
