using Base.Actors;
using Gruntz.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using static Gruntz.Abilities.AbilityPlayer;

namespace Gruntz.Abilities
{
    public class LongAbilityDef : AbilityDef
    {
        public enum AbilityEffect
        {
            HoleDug,
            MaterialSweeped
        }
        public float ExecutionTime = 2.0f;
        public AbilityEffect Effect = AbilityEffect.HoleDug;

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
                switch (Effect) {
                    case AbilityEffect.HoleDug:
                        gameplayManager.HandleGameplayEvent(new HoleDugGameplayEvent {
                            Ability = this,
                            SourceActor = actor,
                            TargetActor = targetActor
                        });
                        break;
                    case AbilityEffect.MaterialSweeped:
                        gameplayManager.HandleGameplayEvent(new MaterialSweepedGameplayEvent { 
                            Ability = this,
                            SourceActor = actor,
                            TargetActor = targetActor
                        });
                        break;
                }
                
                yield return new ExecutionState {
                    GeneralState = GeneralExecutionState.Finished,
                    AnimationState = AnimationExecutionState.AnimationNotPlaying,
                };
            }

            return new AbilityExecution { Coroutine = crt(), OnFinishedCallback = ctx.OnFinished };
        }
    }
}
