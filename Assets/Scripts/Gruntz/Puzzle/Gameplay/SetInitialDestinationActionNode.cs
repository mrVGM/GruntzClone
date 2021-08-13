using Gruntz.Navigation;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.Gameplay
{
    public class SetInitialDestinationActionNode : MonoBehaviour, IActionNode
    {
        public StatusDef StatusAdded;
        public void ExecuteAction(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var interestingEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => x.OperationExecuted == StatusGameplayEvent.Operation.Added
                            && x.Status.StatusDef == StatusAdded);

            foreach (var eventOfInterest in interestingEvents)
            {
                var arrowStatusData = eventOfInterest.Status.Data as ArrowStatusData;
                var actor = eventOfInterest.Actor;
                var actorComponent = actor.ActorComponent;

                var navAgent = actor.GetComponent<NavAgent>();
                navAgent.Target = 1000.0f * (actorComponent.transform.rotation * Vector3.forward) + actor.Pos;
            }
        }
    }
}
