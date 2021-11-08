using Base.Actors;
using Gruntz.Actors;
using Base.Status;
using System.Collections.Generic;
using System.Linq;
using Gruntz.Statuses;
using Gruntz.Gameplay.Actions;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class ActorsClashedGenerator : GameplayActionGenerator
    {
        public ActorTemplateDef GraveDeployDef;
        public ActorInstanceHolderStatusDef ActorInstanceHolderStatusDef;
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
                yield return new KillActorAction {
                    Actor = actorToKill,
                    Reason = KillActorAction.DeathReason.Clash
                };
            }
        }
    }
}
