using Gruntz.Puzzle.Actions;
using Gruntz.Puzzle.Statuses;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using static Base.Navigation.NavAgent;

namespace Gruntz.Puzzle.ActionGenarators
{
    public class ActorArrivedAtArrowDestinationGenerator : GameplayActionGenerator
    {
        public StatusDef ActorStatus;
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

                var disableNavObstaclesStatuses = statusComponent
                    .GetStatuses(x => x.StatusData is DisableNavObstaclesStatusData)
                    .Where(x => {
                        var data = x.StatusData as DisableNavObstaclesStatusData;
                        return data.AssociatedStatusId == arrowStatus.StatusData.StatusId;
                    });

                var overrideActorControllerStatuses = statusComponent
                    .GetStatuses(x => x.StatusData is OverrideActorControllerStatusData)
                    .Where(x => {
                        var data = x.StatusData as OverrideActorControllerStatusData;
                        return data.AssociatedStatusId == arrowStatus.StatusData.StatusId;
                    });

                yield return new ActorArrivedAtArrowDestinationAction {
                    Actor = touchedPositionEvent.Actor,
                    StatusesToRemove = disableNavObstaclesStatuses
                    .Concat(overrideActorControllerStatuses)
                    .Append(arrowStatus).ToArray()
                };
            }
        }
    }
}
