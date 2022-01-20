using System.Collections.Generic;
using Base;
using Base.UI;
using Gruntz.TriggerBox;
using LevelResults;

namespace Gruntz.UI
{
    public class ShowRetryNotification : CoroutineProcess
    {
        public ProcessContextTagDef LevelResultTagDef;
        public Notification Notification;
        public NotificationDataBehaviour.Notification[] RetryNotifications;
        protected override IEnumerator<object> Crt()
        {
            var levelResult = (PuzzleLevelResult.Result) context.GetItem(LevelResultTagDef);
            if (levelResult == PuzzleLevelResult.Result.Completed) {
                yield break;
            }

            bool giveUp = false;
            Notification.GiveUpButton.onClick.RemoveAllListeners();
            Notification.GiveUpButton.onClick.AddListener(() => { giveUp = true; });
            
            Notification.RetryButton.onClick.RemoveAllListeners();
            Notification.RetryButton.onClick.AddListener(() => {
                var game = Game.Instance;
                game.LoadLevel(game.currentLevel, () => { });
            });
            
            Notification.Show(RetryNotifications);
            while (!giveUp) {
                yield return true;
            }
            Notification.GiveUpButton.onClick.RemoveAllListeners();
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
