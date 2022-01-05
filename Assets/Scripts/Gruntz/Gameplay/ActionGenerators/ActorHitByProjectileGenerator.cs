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
                var projectile = actorHitEvent.ProjectileActor.GetComponent<Projectile.ProjectileComponent>();
                var data = projectile.Data as Projectile.ProjectileComponentData;

                yield return new DamageActorAction { Actor = actor, DamageValue = data.DamageAmount, DamageDealer = actorHitEvent.ProjectileActor };
                yield return new PushActorAction { Actor = actor, ProjectileActor = actorHitEvent.ProjectileActor };
            }
        }
    }
}
