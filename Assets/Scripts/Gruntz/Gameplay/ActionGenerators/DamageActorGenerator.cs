using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;
using Gruntz.Actors;
using Gruntz.Statuses;
using Base.Status;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class DamageActorGenerator : GameplayActionGenerator
    {
        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var damageActorEvents = gameplayEvents.OfType<DamageActorGameplayEvent>();

            foreach (var damageActorEvent in damageActorEvents) {
                var actorToDamage = damageActorEvent.TargetActor;
                var statusComponent = actorToDamage.GetComponent<StatusComponent>();
                var healhStatus = statusComponent.GetStatuses(x => x.StatusDef is HealthStatusDef).FirstOrDefault();
                var healthStatusData = healhStatus.StatusData as HealthStatusData;
                float health = healthStatusData.Health;
                health -= damageActorEvent.Ability.DamageAmount;

                if (health <= 0) {
                    yield return new KillActorAction { Actor = actorToDamage, ActorHolderStatusDef = ActorHolderStatusDef, GraveDef = GraveDef };
                    continue;
                }

                yield return new DamageActorAction {
                    DamageValue = damageActorEvent.Ability.DamageAmount,
                    Actor = actorToDamage
                };
            }
        }
    }
}
