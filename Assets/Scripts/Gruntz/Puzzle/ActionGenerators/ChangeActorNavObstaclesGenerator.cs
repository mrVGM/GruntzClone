using Base.MessagesSystem;
using Gruntz.Puzzle.Actions;
using Gruntz.Puzzle.Statuses;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.ActionGenarators
{
    public class ChangeActorNavObstaclesGenerator : GameplayActionGenerator
    {
        public StatusDef ActorStatus;
        public DisableNavObstaclesStatusDef DisableNavObstaclesStatus;

        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var statusAppliedEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => {
                    var statusComponent = x.Actor.GetComponent<StatusComponent>();
                    if (statusComponent.GetStatus(ActorStatus) == null) {
                        return false;
                    }
                    if (x.Status.StatusDef != DisableNavObstaclesStatus) {
                        return false;
                    }
                    return true;
                });

            foreach (var statusAppliedEvent in statusAppliedEvents) {
                yield return new ChangeActorNavObstaclesAction {
                    Actor = statusAppliedEvent.Actor,
                    Disable = statusAppliedEvent.OperationExecuted == StatusGameplayEvent.Operation.Added,
                    DisableNavObstaclesStatus = DisableNavObstaclesStatus
                };
            }
        }
    }
}
