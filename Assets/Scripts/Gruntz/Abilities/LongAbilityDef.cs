using Base.Actors;
using Gruntz.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class LongAbilityDef : AbilityDef
    {
        public float ExecutionTime = 2.0f;
        public override IEnumerator<object> Execute(Actor actor, object target)
        {
            var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
            float startTime = Time.time;
            while (Time.time - startTime < ExecutionTime) {
                yield return null;
            }
            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            gameplayManager.HandleGameplayEvent(new HoleDugGameplayEvent {
                Ability = this,
                SourceActor = actor,
                TargetActor = target as Actor
            });
        }
    }
}
