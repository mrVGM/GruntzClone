using System.Collections.Generic;
using Base.UI;
using LevelResults;
using UnityEngine;

namespace Gruntz.UI
{
    public class ShowWinLoseNotification : CoroutineProcess
    {
        public Animator WinNotification;
        public Animator LoseNotification;
        public ProcessContextTagDef LevelResultTagDef;
        
        protected override IEnumerator<object> Crt()
        {
            var result = (PuzzleLevelResult.Result) context.GetItem(LevelResultTagDef);
            if (result == PuzzleLevelResult.Result.Completed) {
                WinNotification.SetTrigger("Shown");
            }
            if (result == PuzzleLevelResult.Result.Failed) {
                LoseNotification.SetTrigger("Shown");
            }
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
