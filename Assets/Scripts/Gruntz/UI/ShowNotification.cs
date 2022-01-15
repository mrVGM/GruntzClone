using System.Collections.Generic;
using Base.UI;
using Gruntz.TriggerBox;

namespace Gruntz.UI
{
    public class ShowNotification : CoroutineProcess
    {
        public ProcessContextTagDef NotificationMessagesTagDef;
        public Notification Notification;
        protected override IEnumerator<object> Crt()
        {
            var notificationData = context.GetItem(NotificationMessagesTagDef) as IEnumerable<NotificationDataBehaviour.Notification>;
            Notification.Show(notificationData);
            while (true) {
                yield return true;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            Notification.Hide();
            yield break;
        }
    }
}
