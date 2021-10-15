using Base.Actors;
using Gruntz.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class DigHoleAbilityDef : AbilityDef
    {
        public override IEnumerator<object> Execute(Actor actor, object target)
        {
            var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
            float startTime = Time.time;
            while (abilitiesComponent.IsOnCooldown(this)) {
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
