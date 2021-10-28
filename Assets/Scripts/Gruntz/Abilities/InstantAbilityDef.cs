using Base.Actors;
using Base.MessagesSystem;
using Gruntz.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Abilities.AbilityPlayer;

namespace Gruntz.Abilities
{
    public class InstantAbilityDef : AbilityDef
    {
        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            var targetActor = ctx.Target as Actor;
            IEnumerator<ExecutionState> crt()
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
                        yield return new ExecutionState {
                            GeneralState = GeneralExecutionState.Finished,
                            AnimationState = AnimationExecutionState.AnimationNotPlaying,
                        };
                        yield break;
                    }

                    if (eventsForMe().Any(x => x.stringParameter == "ActionEnd"))
                    {
                        yield return new ExecutionState {
                            GeneralState = GeneralExecutionState.Finished,
                            AnimationState = AnimationExecutionState.AnimationNotPlaying,
                        };
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
                    yield return new ExecutionState {
                        GeneralState = GeneralExecutionState.Playing,
                        AnimationState = AnimationExecutionState.AnimationPlaying,
                    };
                }
            }

            return new AbilityExecution { Coroutine = crt(), OnFinishedCallback = ctx.OnFinished };
        }
    }
}
