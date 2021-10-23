using System.Collections.Generic;
using Base;
using Base.UI;
using UnityEngine;
using static Base.MainUpdaterLock;

namespace Gruntz.UI
{
    public class PauseGame : CoroutineProcess
    {
        public ExecutionOrderTagDef OrderTagDef;
        public List<ILock> Locks = new List<ILock>();
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            Locks.Clear();
            foreach (var tag in game.MainUpdater.ExecutionOrder)
            {
                if (tag == OrderTagDef)
                {
                    continue;
                }
                var l = game.MainUpdater.MainUpdaterLock.TryLock(tag);
                Locks.Add(l);
            }
            while (true) {
                yield return true;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            var game = Game.Instance;
            foreach (var l in Locks) {
                game.MainUpdater.MainUpdaterLock.Unlock(l);
            }
            yield break;
        }
    }
}
