using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;
using Gruntz.Actors;
using Gruntz.Statuses;
using Base.Status;
using Base.Gameplay;

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
                yield return new DamageActorAction {
                    DamageValue = damageActorEvent.Ability.DamageAmount,
                    Actor = actorToDamage,
                    DamageDealer = damageActorEvent.SourceActor,
                };
            }
        }
    }
}
