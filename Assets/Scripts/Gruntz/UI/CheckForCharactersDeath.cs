using Base;
using Base.Actors;
using Base.Status;
using System.Collections.Generic;
using Base.UI;
using System.Linq;
using LevelResults;

namespace Gruntz.UI
{
    public class CheckForCharactersDeath : CoroutineProcess
    {
        public StatusDef RegularActor;
        public ProcessContextTagDef LevelResultTagDef;
        protected override IEnumerator<object> Crt()
        {
            var actorManager = ActorManager.GetActorManagerFromContext();

            while (true) {
                var activeActors = actorManager.Actors.Where(x => {
                    var statusComponent = x.GetComponent<StatusComponent>();
                    return statusComponent.GetStatus(RegularActor) != null;
                });

                if (!activeActors.Any()) {
                    context.PutItem(LevelResultTagDef, PuzzleLevelResult.Result.Failed);
                    yield break;
                }

                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
