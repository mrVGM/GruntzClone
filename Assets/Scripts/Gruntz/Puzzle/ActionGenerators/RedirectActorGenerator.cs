using Gruntz.Puzzle.Actions;
using Gruntz.Puzzle.Statuses;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.ActionGenarators
{
    public class RedirectActorGenerator : GameplayActionGenerator
    {
        public StatusDef ActorStatus;
        public ArrowStatusDef ArrowStatus;
        public OverrideActorControllerStatusDef OverrideActorControllerStatusDef;
        public DisableNavObstaclesStatusDef DisableNavObstaclesStatusDef;

        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var statusAppliedEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => {
                    if (x.OperationExecuted != StatusGameplayEvent.Operation.Added)
                    {
                        return false;
                    }
                    var statusComponent = x.Actor.GetComponent<StatusComponent>();
                    if (statusComponent.GetStatus(ActorStatus) == null)
                    {
                        return false;
                    }
                    if (x.Status.StatusDef != ArrowStatus)
                    {
                        return false;
                    }
                    return true;
                });

            foreach (var statusAppliedEvent in statusAppliedEvents) {
                var actor = statusAppliedEvent.Actor;
                var actorComponent = actor.ActorComponent;
                var arrowStatusData = statusAppliedEvent.Status.Data as ArrowStatusData;
                var changeActorControllerStatusData = OverrideActorControllerStatusDef.Data as OverrideActorControllerStatusData;
                changeActorControllerStatusData.AssociatedStatusId = arrowStatusData.StatusId;
                var disableNavObstaclesStatusData = DisableNavObstaclesStatusDef.Data as DisableNavObstaclesStatusData;
                disableNavObstaclesStatusData.AssociatedStatusId = arrowStatusData.StatusId;

                yield return new RedirectActorAction {
                    Actor = actor,
                    Destination = arrowStatusData.Destination,
                    StatusesToAdd = new[] { changeActorControllerStatusData.CreateStatus(), disableNavObstaclesStatusData.CreateStatus() }
                };
            }
        }
    }
}
