using System.Collections.Generic;
using Base;
using Base.UI;
using LevelResults;

namespace Gruntz.UI
{
    public class LoadLevelSelectionLevel : CoroutineProcess
    {
        public LevelDef LevelSelectionLevelDef;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            game.LoadLevel(LevelSelectionLevelDef, () => { 
                var levelResult = LevelResultHolder.GetLevelResultHolderFromContext();
                levelResult.Level = game.currentLevel;
                levelResult.LevelResult = PuzzleLevelResult.Result.Completed;
            });
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
