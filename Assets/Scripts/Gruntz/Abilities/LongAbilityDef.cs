using Base.Actors;
using Gruntz.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class LongAbilityDef : AbilityDef
    {
        public float ExecutionTime = 2.0f;

        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            var targetActor = ctx.Target as Actor;

            IEnumerator<object> crt()
            {
                var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
                float startTime = Time.time;
                while (Time.time - startTime < ExecutionTime)
                {
                    yield return null;
                }
                var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                gameplayManager.HandleGameplayEvent(new HoleDugGameplayEvent
                {
                    Ability = this,
                    SourceActor = actor,
                    TargetActor = targetActor
                });
            }

            return new AbilityExecution { Coroutine = crt(), OnFinishedCallback = ctx.OnFinished };
        }
    }
}
