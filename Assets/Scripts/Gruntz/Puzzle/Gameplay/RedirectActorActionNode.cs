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
    public class RedirectActorActionNode : MonoBehaviour, IActionNode
    {
        public StatusDef ActorStatus;
        public StatusDef StatusAdded;
        public MessagesBoxTagDef UnitControllerChannelTagDef;
        public void ExecuteAction(IEnumerable<GameplayEvent> gameplayEvents)
        {
            void redirect(Actor unit, Vector3 destination)
            {
                var unitController = unit.GetComponent<UnitController>();
                unitController.MessagesBox = UnitControllerChannelTagDef;

                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                var moveToPositionIntruction = new MoveToPosition { Position = destination };
                messagesSystem.SendMessage(UnitControllerChannelTagDef, this, new UnitControllerInstruction
                {
                    Unit = unit,
                    Executable = moveToPositionIntruction,
                });
            }

            var interestingEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => x.OperationExecuted == StatusGameplayEvent.Operation.Added
                            && x.Actor.GetComponent<StatusComponent>().GetStatus(ActorStatus) != null
                            && x.Status.StatusDef == StatusAdded);

            foreach (var eventOfInterest in interestingEvents)
            {
                var arrowStatusData = eventOfInterest.Status.Data as ArrowStatusData;
                redirect(eventOfInterest.Actor, arrowStatusData.Destination);
            }
        }
    }
}
