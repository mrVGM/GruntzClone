using Base.MessagesSystem;
using Gruntz.Actors;
using Gruntz.Navigation;
using Gruntz.Status;
using Gruntz.UserInteraction.UnitController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.Gameplay
{
    public class RedirectBallActorActionNode : MonoBehaviour, IActionNode
    {
        public StatusDef ActorStatus;
        public StatusDef StatusAdded;
        public void ExecuteAction(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var interestingEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => x.OperationExecuted == StatusGameplayEvent.Operation.Added
                            && x.Actor.GetComponent<StatusComponent>().GetStatus(ActorStatus) != null
                            && x.Status.StatusDef == StatusAdded);

            foreach (var eventOfInterest in interestingEvents)
            {
                var arrowStatusData = eventOfInterest.Status.Data as ArrowStatusData;
                var ballActor = eventOfInterest.Actor;
                var navAgent = ballActor.GetComponent<NavAgent>();
                navAgent.Target = 1000.0f * ((Vector3)arrowStatusData.Destination - (Vector3)arrowStatusData.Anchor) + (Vector3)arrowStatusData.Anchor;
            }
        }
    }
}
