using System.Collections.Generic;
using System.Linq;
using Base;
using Base.UI;

namespace Gruntz.UI
{
    public class LoadLevel : CoroutineProcess
    {
        public TagDef SaveTagDef;
        public ProcessContextTagDef LevelResultTagDef;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == SaveTagDef);
            var currentLevel = game.currentLevel;
            savesManager.LoadSave(save, () => {
                var levelResult = LevelResultHolder.GetLevelResultHolderFromContext();
                levelResult.Level = currentLevel;
                levelResult.LevelResult = context.GetItem(LevelResultTagDef);
            });
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
