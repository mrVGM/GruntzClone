using Base;
using Base.Actors;
using Base.Navigation;
using Gruntz.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.AI
{
    public class MoveInMeleeRangeAndExecuteAbility : IAIAction
    {
        IEnumerator<object> _crt;
        public MoveInMeleeRangeAndExecuteAbility(Actor actor, Actor targetActor, AbilityDef ability)
        {
            var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
            abilitiesComponent.IsEnabled(ability);
            var navAgent = actor.GetComponent<NavAgent>();
            var navigation = Navigation.GetNavigationFromContext();
            var map = navigation.Map;
        }
        public bool CanProceed()
        {
            return true;
        }

        public void Update()
        {
        }

        public void Stop()
        {
        }
    }
}
