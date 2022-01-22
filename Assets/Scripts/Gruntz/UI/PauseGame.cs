using System.Collections.Generic;
using System.Linq;
using Base;
using Base.UI;
using static Base.MainUpdaterLock;

namespace Gruntz.UI
{
    public class PauseGame : CoroutineProcess
    {
        public bool Pause = true;
        public ExecutionOrderTagDef[] OrderTagsToLeaveRunning;
        public ProcessContextTagDef CreatedLocksTagDef;
        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            List<ILock> locks;
            if (Pause)
            {
                locks = new List<ILock>();
                foreach (var tag in game.MainUpdater.ExecutionOrder) {
                    if (OrderTagsToLeaveRunning.Contains(tag)) {
                        continue;
                    }
                    var l = game.MainUpdater.MainUpdaterLock.TryLock(tag);
                    locks.Add(l);
                }

                context.PutItem(CreatedLocksTagDef, locks);
                yield break;
            }

            locks = context.GetItem(CreatedLocksTagDef) as List<ILock>;
            context.PutItem(CreatedLocksTagDef, null);

            foreach (var l in locks) {
                game.MainUpdater.MainUpdaterLock.Unlock(l);
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
