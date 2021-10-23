using System.Collections.Generic;
using Base.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Gruntz.UI
{
    public class ShowNotification : CoroutineProcess
    {
        public ProcessContextTagDef NotificationMessagesTagDef;
        public GameObject ObjectToShow;
        public Text NotificationText;
        protected override IEnumerator<object> Crt()
        {
            string notificationText = context.GetItem(NotificationMessagesTagDef) as string;
            NotificationText.text = notificationText;
            ObjectToShow.SetActive(true);
            while (true) {
                yield return true;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            ObjectToShow.SetActive(false);
            yield break;
        }
    }
}
