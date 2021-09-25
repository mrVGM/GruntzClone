using Gruntz.Actors;
using Gruntz.Puzzle.Actions;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Puzzle.ActionGenarators
{
    public class ActorsClashedGenerator : GameplayActionGenerator
    {
        public StatusDef[] Hardness;
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

                int hardness(Actor actor) {
                    var statusComponent = actor.GetComponent<StatusComponent>();
                    if (statusComponent == null) {
                        return 200000;
                    }
                    for (int i = Hardness.Length - 1; i >= 0; --i) {
                        if (statusComponent.GetStatus(Hardness[i]) != null) {
                            return i;
                        }
                    }
                    return 100000;
                }

                var actorToKill = actorsCollection(clash).OrderBy(hardness).FirstOrDefault();
                yield return new KillActorAction { Actor = actorToKill };
            }
        }
    }
}
