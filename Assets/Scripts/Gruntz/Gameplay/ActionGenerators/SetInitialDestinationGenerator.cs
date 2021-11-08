using Gruntz.Gameplay.Actions;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Base.Status.StatusComponent;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class SetInitialDestinationGenerator : GameplayActionGenerator
    {
        public StatusDef InitialDestinationStatus;
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var statusAppliedEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => x.OperationExecuted == StatusGameplayEvent.Operation.Added && x.Status.StatusDef == InitialDestinationStatus);

            foreach (var statusAppliedEvent in statusAppliedEvents) {
                var actor = statusAppliedEvent.Actor;
                var actorComponent = actor.ActorComponent;
                yield return new SetInitialDestinationAction { 
                    Actor = actor,
                    StatusToRemove = statusAppliedEvent.Status,
                    Destination = 1000.0f * (actorComponent.transform.rotation * Vector3.forward) + actor.Pos
                };
            }
        }
    }
}
