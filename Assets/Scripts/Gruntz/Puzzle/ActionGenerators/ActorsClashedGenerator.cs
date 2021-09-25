using Gruntz.Actors;
using Gruntz.Puzzle.Actions;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.ActionGenarators
{
    public class ActorsClashedGenerator : GameplayActionGenerator
    {
        public StatusDef BallActor;
        public StatusDef Actor;
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var actorsClashedGameplayEvents = gameplayEvents.OfType<ActorsClashedGameplayEvent>();
            var uniqueClashes = new List<ActorsClashedGameplayEvent>();
            IEnumerable<Actor> actorsCollection(ActorsClashedGameplayEvent clashedActors)
            {
                yield return clashedActors.Actor1;
                yield return clashedActors.Actor2;
            }

            foreach (var actorsClashedEvent in actorsClashedGameplayEvents) {
                if (uniqueClashes.Any(x => {
                    

                    bool different = actorsCollection(actorsClashedEvent).Except(actorsCollection(x)).Any();
                    return !different;
                })) {
                    continue;
                }

                uniqueClashes.Add(actorsClashedEvent);
            }
            foreach (var clash in uniqueClashes) {
                var actors = actorsCollection(clash);
                var ballActor = actors.FirstOrDefault(x => {
                    var statusComponent = x.GetComponent<StatusComponent>();
                    return statusComponent.GetStatus(BallActor) != null;
                });
                var regualActor = actors.FirstOrDefault(x => {
                    var statusComponent = x.GetComponent<StatusComponent>();
                    return statusComponent.GetStatus(Actor) != null;
                });

                if (ballActor != null && regualActor != null) {
                    yield return new KillActorAction { Actor = regualActor };
                }
            }
        }
    }
}
