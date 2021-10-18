using System.Collections.Generic;
using System.Linq;
using Base;
using Base.UI;

namespace Gruntz.UI
{
    public class LoadLevel : CoroutineProcess
    {
        public TagDef SaveTagDef;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.FirstOrDefault(x => x.SaveTag == SaveTagDef);
            savesManager.LoadSave(save);
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
