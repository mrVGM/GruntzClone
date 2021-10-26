using Base.Actors;
using Base.MessagesSystem;
using Gruntz.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class InstantAttackAbilityDef : AbilityDef, IAttackAbility
    {
        public float Damage = 10;
        public float DamageAmount => Damage;

        public override IEnumerator<object> Execute(Actor actor, object target)
        {
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            IEnumerable<AnimationEvent> eventsForMe()
            {
                var messages = messagesSystem.GetMessages(AnimationEventMessages);
                messages = messages.ToList();
                messages = messages.Where(x => x.Sender == actor);
                return messages.Select(x => x.Data as AnimationEvent);
            }
            var targetActor = target as Actor;
            while (true)
            {
                if (!actor.IsInPlay)
                {
                    yield break;
                }

                if (eventsForMe().Any(x => x.stringParameter == "ActionEnd"))
                {
                    yield break;
                }

                if (eventsForMe().Any(x => x.stringParameter == "Hit"))
                {
                    var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                    gameplayManager.HandleGameplayEvent(new DamageActorGameplayEvent
                    {
                        Ability = this,
                        SourceActor = actor,
                        TargetActor = targetActor
                    });
                }
                yield return null;
            }
        }
    }
}
