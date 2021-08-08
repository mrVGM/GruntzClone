using System.Collections.Generic;
using System.Linq;
using Base.MessagesSystem;
using Gruntz.Actors;
using Gruntz.UserInteraction.UnitController;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle
{
    public class SteppedOnArrowGameplayEventHandlerDef : GameplayEventHandlerDef
    {
        public ArrowStatusDef SteppedOnArrowStatus;
        public MessagesBoxTagDef UnitControllerChannelTagDef;

        private IEnumerable<StatusGameplayEvent> GetInterestingEvents(IEnumerable<GameplayEvent> gameplayEvents)
        {
            return gameplayEvents.OfType<StatusGameplayEvent>().Where(x => x.Status.StatusDef == SteppedOnArrowStatus);
        }
        public override int GetPriority(IEnumerable<GameplayEvent> gameplayEvents)
        {
            if (GetInterestingEvents(gameplayEvents).Any())
            {
                return 10;
            }
            return -1;
        }
        public override void HandleEvents(IEnumerable<GameplayEvent> gameplayEvents)
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
            void restoreUnitController(Actor unit, MessagesBoxTagDef previousUnitControllerChannel)
            {
                var unitController = unit.GetComponent<UnitController>();
                unitController.MessagesBox = previousUnitControllerChannel;
            }

            var eventsOfInterest = GetInterestingEvents(gameplayEvents);
            foreach (var eventOfInterest in eventsOfInterest)
            {
                var arrowStatusData = eventOfInterest.Status.Data as ArrowStatusData;
                if (eventOfInterest.OperationExecuted == StatusGameplayEvent.Operation.Added)
                {
                    redirect(eventOfInterest.Actor, arrowStatusData.Destination);
                    return;
                }
                if (eventOfInterest.OperationExecuted == StatusGameplayEvent.Operation.Removed)
                {
                    restoreUnitController(eventOfInterest.Actor, arrowStatusData.PreviousUnitControllerChannel);
                    return;
                }
            }
        }
    }
}
