using Base.Actors;
using Gruntz.Abilities;

namespace Gruntz.UnitController
{
    public struct ExecuteAbility : IUnitExecutable
    {
        public HitAbilityDef Ability;
        public object Target;

        void IUnitExecutable.Execute(Actor actor)
        {
            var abilityManager = AbilityManager.GetAbilityManagerFromContext();
            abilityManager.AbilityPlayers.Add(new AbilityPlayer(Ability, actor, Target));
        }
    }
}
