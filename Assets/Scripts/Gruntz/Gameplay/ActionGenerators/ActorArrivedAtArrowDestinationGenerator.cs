using Gruntz.Gameplay.Actions;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using static Base.Navigation.NavAgent;
using Gruntz.Statuses;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class ActorArrivedAtArrowDestinationGenerator : GameplayActionGenerator
    {
        public ArrowStatusDef ArrowStatus;

        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var touchedPositionEvents = gameplayEvents.OfType<ActorTouchedPositionGameplayEvent>();

            foreach (var touchedPositionEvent in touchedPositionEvents) {
                var statusComponent = touchedPositionEvent.Actor.GetComponent<StatusComponent>();
                if (statusComponent.GetStatus(ArrowStatus) == null) {
                    continue;
                }
                var arrowStatuses = statusComponent.GetStatuses(x => {
                    var arrowStatusData = x.StatusData as ArrowStatusData;
                    if (arrowStatusData == null) {
                        return false;
                    }
                    return touchedPositionEvent.Positions != null
                            && touchedPositionEvent.Positions.Any(y => (y - arrowStatusData.Destination).sqrMagnitude < 0.01f);
                });

                var arrowStatus = arrowStatuses.FirstOrDefault();
                if (arrowStatus == null) {
                    continue;
                }

                yield return new RemoveArrowStatusAction {
                    Actor = touchedPositionEvent.Actor,
                    ArrowStatus = arrowStatus,
                };
            }
        }
    }
}
