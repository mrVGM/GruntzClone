using Base.UI;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LevelSelection.UserInteraction
{
    public class UpdateAreaLabel : CoroutineProcess
    {
        public Text Text;
        protected override IEnumerator<object> Crt()
        {
            var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();
            while (true) {
                Text.text = levelSelectionMap.CurArea.Label;
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
