using Base.Actors;
using Gruntz.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using static Gruntz.Abilities.AbilityPlayer;

namespace Gruntz.Abilities
{
    public class LongAbilityDef : AbilityDef
    {
        public float ExecutionTime = 2.0f;

        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            var targetActor = ctx.Target as Actor;

            IEnumerator<ExecutionState> crt()
            {
                var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
                float startTime = Time.time;
                while (Time.time - startTime < ExecutionTime)
                {
                    yield return new ExecutionState {
                        GeneralState = GeneralExecutionState.Playing,
                        AnimationState = AnimationExecutionState.AnimationPlaying,
                    };
                }
                var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                gameplayManager.HandleGameplayEvent(new HoleDugGameplayEvent
                {
                    Ability = this,
                    SourceActor = actor,
                    TargetActor = targetActor
                });
                yield return new ExecutionState {
                    GeneralState = GeneralExecutionState.Finished,
                    AnimationState = AnimationExecutionState.AnimationNotPlaying,
                };
            }

            return new AbilityExecution { Coroutine = crt(), OnFinishedCallback = ctx.OnFinished };
        }
    }
}
