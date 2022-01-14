using System.Collections.Generic;
using Base.UI;
using UnityEngine.UI;

namespace Gruntz.UI
{
    public class WaitForNotificationClosed : CoroutineProcess
    {
        public Notification Notification;
        public Button DismissNotificationButton;
        protected override IEnumerator<object> Crt()
        {
            DismissNotificationButton.onClick.RemoveAllListeners();
            bool dismissClicked = false;
            DismissNotificationButton.onClick.AddListener(() => {
                Notification.DismissNotification();
                dismissClicked = true;
                DismissNotificationButton.onClick.RemoveAllListeners();
            });

            while (!dismissClicked) {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
