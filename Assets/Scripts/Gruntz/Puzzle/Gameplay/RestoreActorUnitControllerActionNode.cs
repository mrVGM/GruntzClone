using Base.MessagesSystem;
using Gruntz.Actors;
using Gruntz.Status;
using Gruntz.UserInteraction.UnitController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.Gameplay
{
    public class RestoreActorUnitControllerActionNode : MonoBehaviour, IActionNode
    {
        public StatusDef ActorStatus;
        public StatusDef StatusAdded;
        public void ExecuteAction(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var interestingEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => x.OperationExecuted == StatusGameplayEvent.Operation.Removed
                            && x.Actor.GetComponent<StatusComponent>().GetStatus(ActorStatus) != null
                            && x.Status.StatusDef == StatusAdded);

            foreach (var eventOfInterest in interestingEvents)
            {
                var arrowStatusData = eventOfInterest.Status.Data as ArrowStatusData;
                var unitController = eventOfInterest.Actor.GetComponent<UnitController>();
                unitController.MessagesBox = arrowStatusData.PreviousUnitControllerChannel;
            }
        }
    }
}
