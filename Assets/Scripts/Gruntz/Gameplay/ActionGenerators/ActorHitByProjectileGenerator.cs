using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class ActorHitByProjectileGenerator : GameplayActionGenerator
    {
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var actorHitEvents = gameplayEvents.OfType<ProjectileHitActorGameplayEvent>();
            foreach (var actorHitEvent in actorHitEvents) {
                var actor = actorHitEvent.ActorHit;
                yield return new PushActorAction { Actor = actor };
            }
        }
    }
}
