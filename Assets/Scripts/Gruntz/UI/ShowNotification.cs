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
            var notificationText = context.GetItem(NotificationMessagesTagDef) as TriggerShowNotificationActionDef.Notification;
            Notification.Show(notificationText.NotificationText, notificationText.Video);
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
