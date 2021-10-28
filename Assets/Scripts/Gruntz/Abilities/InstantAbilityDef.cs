using Base.Actors;
using Base.MessagesSystem;
using Gruntz.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class InstantAbilityDef : AbilityDef
    {
        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            var targetActor = ctx.Target as Actor;
            IEnumerator<AbilityProgress> crt()
            {
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                IEnumerable<AnimationEvent> eventsForMe()
                {
                    var messages = messagesSystem.GetMessages(AnimationEventMessages);
                    messages = messages.ToList();
                    messages = messages.Where(x => x.Sender == actor);
                    return messages.Select(x => x.Data as AnimationEvent);
                }
                while (true)
                {
                    if (!actor.IsInPlay)
                    {
                        yield return AbilityProgress.Finished;
                        yield break;
                    }

                    if (eventsForMe().Any(x => x.stringParameter == "ActionEnd"))
                    {
                        yield return AbilityProgress.Finished;
                        yield break;
                    }

                    if (eventsForMe().Any(x => x.stringParameter == "Hit"))
                    {
                        var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                        gameplayManager.HandleGameplayEvent(new DestroyedActorGameplayEvent
                        {
                            Ability = this,
                            SourceActor = actor,
                            TargetActor = targetActor
                        });
                    }
                    yield return AbilityProgress.PlayingAnimation;
                }
            }

            return new AbilityExecution { Coroutine = crt(), OnFinishedCallback = ctx.OnFinished };
        }
    }
}
