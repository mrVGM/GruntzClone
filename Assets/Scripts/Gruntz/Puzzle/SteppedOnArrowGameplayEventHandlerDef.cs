using System.Collections.Generic;
using System.Linq;
using Base.MessagesSystem;
using Gruntz.Status;
using Gruntz.UserInteraction.UnitController;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle
{
    public class SteppedOnArrowGameplayEventHandlerDef : GameplayEventHandlerDef
    {
        public StatusDef SteppedOnArrowStatus;
        public MessagesBoxTagDef UnitControllerChannelTagDef;

        private IEnumerable<StatusGameplayEvent> GetInterestingEvents(IEnumerable<GameplayEvent> gameplayEvents)
        {
            return gameplayEvents.OfType<StatusGameplayEvent>().Where(x => x.Status.StatusDef == SteppedOnArrowStatus && x.OperationExecuted == StatusGameplayEvent.Operation.Added);
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
            var eventsOfInterest = GetInterestingEvents(gameplayEvents);
            foreach (var eventOfInterest in eventsOfInterest)
            {
                var unitController = eventOfInterest.Actor.GetComponent<UnitController>();
                unitController.MessagesBox = UnitControllerChannelTagDef;

                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();

                var moveToPositionIntruction = new MoveToPosition { Position = new UnityEngine.Vector3(3.5f, 0.0f, 3.5f) };
                messagesSystem.SendMessage(UnitControllerChannelTagDef, this, new UnitControllerInstruction
                {
                    Unit = eventOfInterest.Actor,
                    Executable = moveToPositionIntruction,
                });
            }
        }
    }
}
