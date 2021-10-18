using System.Collections.Generic;
using Base;
using Base.UI;

namespace Gruntz.UI
{
    public class LoadLevel : CoroutineProcess
    {
        public LevelDef LevelDef;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            game.LoadLevel(LevelDef, () => { });
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
