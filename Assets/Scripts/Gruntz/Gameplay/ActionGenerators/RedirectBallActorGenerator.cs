using Gruntz.Gameplay.Actions;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Base.Status.StatusComponent;
using Gruntz.Statuses;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class RedirectBallActorGenerator : GameplayActionGenerator
    {
        public StatusDef BallActorStatus;
        public ArrowStatusDef ArrowStatus;

        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var statusAppliedEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => {
                    if (x.OperationExecuted != StatusGameplayEvent.Operation.Added)
                    {
                        return false;
                    }
                    var statusComponent = x.Actor.GetComponent<StatusComponent>();
                    if (statusComponent.GetStatus(BallActorStatus) == null)
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

                yield return new RedirectBallActorAction {
                    Actor = actor,
                    Destination = 1000.0f * ((Vector3)arrowStatusData.Destination - (Vector3)arrowStatusData.Anchor) + arrowStatusData.Destination,
                    StatusToRemove = statusAppliedEvent.Status
                };
            }
        }
    }
}
