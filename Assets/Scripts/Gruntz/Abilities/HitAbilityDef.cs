using Base.Actors;
using Base.MessagesSystem;
using Gruntz.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class HitAbilityDef : AbilityDef
    {
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
            while (true) {
                if (eventsForMe().Any(x => x.stringParameter == "ActionEnd")) {
                    yield break;
                }

                if (eventsForMe().Any(x => x.stringParameter == "Hit")) {
                    var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                    gameplayManager.HandleGameplayEvent(new DestroyedActorGameplayEvent {
                        Ability = this,
                        SourceActor = actor,
                        TargetActor = target as Actor
                    });
                }
                yield return null;
            }
        }
    }
}
