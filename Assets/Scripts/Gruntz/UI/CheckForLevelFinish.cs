using Base;
using Base.Actors;
using Base.Status;
using System.Collections.Generic;
using Base.UI;
using Gruntz.Items;
using System.Linq;
using Gruntz.Equipment;
using Gruntz.Team;
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
                    if (statusComponent.GetStatus(RegularActor) == null) {
                        return false;
                    }

                    var teamComponent = x.GetComponent<TeamComponent>();
                    if (teamComponent == null) {
                        return false;
                    }

                    return teamComponent.UnitTeam == TeamComponent.Team.Player;
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
