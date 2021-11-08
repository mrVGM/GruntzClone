using Base.MessagesSystem;
using Gruntz.Gameplay.Actions;
using Gruntz.Statuses;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using static Base.Status.StatusComponent;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class OverrideActorControllerGenerator : GameplayActionGenerator
    {
        public StatusDef ActorStatus;
        public OverrideActorControllerStatusDef OverrideActorControllerStatus;
        public MessagesBoxTagDef MessagesBoxTagDef;

        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var statusAppliedEvents = gameplayEvents.OfType<StatusGameplayEvent>()
                .Where(x => {
                    var statusComponent = x.Actor.GetComponent<StatusComponent>();
                    if (statusComponent.GetStatus(ActorStatus) == null) {
                        return false;
                    }
                    if (x.Status.StatusDef != OverrideActorControllerStatus) {
                        return false;
                    }
                    return true;
                });

            foreach (var statusAppliedEvent in statusAppliedEvents) {
                yield return new OverrideActorControllerAction {
                    Actor = statusAppliedEvent.Actor,
                    Restore = statusAppliedEvent.OperationExecuted == StatusGameplayEvent.Operation.Removed,
                    MessagesBoxTagDef = MessagesBoxTagDef
                };
            }
        }
    }
}
