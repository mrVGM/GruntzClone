using Base;
using Base.Actors;
using Base.Status;
using System.Collections.Generic;
using Base.UI;
using Gruntz.Items;
using System.Linq;
using Gruntz.Equipment;
using LevelResults;

namespace Gruntz.UI
{
    public class CheckForLevelFinish : CoroutineProcess
    {
        public ItemDef TrophyItem;
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

                if (activeActors.Any(x => {
                    var equipmentComponent = x.GetComponent<EquipmentComponent>();
                    if (equipmentComponent == null) {
                        return false;
                    }
                    return equipmentComponent.Weapon == TrophyItem;
                })) {
                    context.PutItem(LevelResultTagDef, PuzzleLevelResult.Result.Completed);
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
