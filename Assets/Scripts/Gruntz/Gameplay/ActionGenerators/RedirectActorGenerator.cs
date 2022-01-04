using Gruntz.Gameplay.Actions;
using Gruntz.Statuses;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using static Base.Status.StatusComponent;
using Base.Gameplay;
using UnityEngine;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class RedirectActorGenerator : GameplayActionGenerator
    {
        public StatusDef ActorStatus;
        public ArrowStatusDef ArrowStatus;
        public OverrideActorControllerStatusDef OverrideActorControllerStatusDef;
        public DisableNavObstaclesStatusDef DisableNavObstaclesStatusDef;
        public PushStatusDef PushStatusDef;

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
                var arrowStatusData = statusAppliedEvent.Status.Data as ArrowStatusData;
                var changeActorControllerStatusData = OverrideActorControllerStatusDef.Data as OverrideActorControllerStatusData;
                changeActorControllerStatusData.AssociatedStatusId = arrowStatusData.StatusId;
                var disableNavObstaclesStatusData = DisableNavObstaclesStatusDef.Data as DisableNavObstaclesStatusData;
                disableNavObstaclesStatusData.AssociatedStatusId = arrowStatusData.StatusId;

                var statusComponent = actor.GetComponent<StatusComponent>();
                var pushStatus = statusComponent.GetStatus(PushStatusDef);
                if (pushStatus != null) {
                    var statusData = pushStatus.StatusData as PushStatusData;
                    Vector3 offset = (Vector3)statusData.PushDestination - (Vector3)arrowStatusData.Anchor;
                    if (offset.magnitude > 0.1)  {
                        yield return new RemoveStatusAction() {
                            Actor = actor,
                            Status = statusAppliedEvent.Status,
                        };
                        continue;
                    }
                }

                yield return new RedirectActorAction {
                    Actor = actor,
                    Destination = arrowStatusData.Destination,
                };
                yield return new AddStatusAction {
                    Actor = actor,
                    Status = changeActorControllerStatusData.CreateStatus(),
                };
                yield return new AddStatusAction {
                    Actor = actor,
                    Status = disableNavObstaclesStatusData.CreateStatus(),
                };
            }
        }
    }
}
