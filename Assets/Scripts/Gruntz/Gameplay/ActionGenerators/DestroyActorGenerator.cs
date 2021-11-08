using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;
using Gruntz.Actors;
using Gruntz.Statuses;
using Base.Gameplay;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class DestroyActorGenerator : GameplayActionGenerator
    {
        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var destroyActorEvents = gameplayEvents.OfType<DestroyedActorGameplayEvent>();

            foreach (var destroyedActorGameplayEvent in destroyActorEvents) {
                yield return new KillActorAction { 
                    Actor = destroyedActorGameplayEvent.TargetActor,
                    Reason = KillActorAction.DeathReason.Destruction,
                };
            }
        }
    }
}
