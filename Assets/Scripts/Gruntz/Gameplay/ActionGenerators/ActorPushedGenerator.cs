using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;
using Base.Gameplay;
using static Base.Status.StatusComponent;
using Gruntz.Statuses;
using Base.Status;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class ActorPushedGenerator : GameplayActionGenerator
    {
        public PushStatusDef PushStatusDef;
        public ArrowStatusDef ArrowStatusDef;
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var actorPushEvents = gameplayEvents.OfType<StatusGameplayEvent>();
            foreach (var actorPushEvent in actorPushEvents) {
                var pushStatus = actorPushEvent.Status;
                if (pushStatus.StatusDef != PushStatusDef) {
                    continue;
                }
                if (actorPushEvent.OperationExecuted != StatusGameplayEvent.Operation.Added) {
                    continue;
                }

                var actor = actorPushEvent.Actor;
                var statusComponent = actor.GetComponent<StatusComponent>();
                var arrowStatuses = statusComponent.GetStatuses(x => x.StatusDef == ArrowStatusDef);
                foreach (var arrowStatus in arrowStatuses) {
                    yield return new RemoveArrowStatusAction {
                        Actor = actor,
                        ArrowStatus = arrowStatus,
                    };
                }
            }
        }
    }
}
