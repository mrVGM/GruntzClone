using Base.Actors;
using Gruntz.Abilities;
using Gruntz.UnitController.Instructions;

namespace Gruntz.UnitController
{
    public static class Utils
    {
        public static IUnitExecutable GetAttackInstruction(Actor actor, Actor targetActor)
        {
            var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
            var attackAbility = abilitiesComponent.GetAttackAbility();
            var projectileAttackAbility = attackAbility as ProjectileAttackAbilityDef;

            if (projectileAttackAbility != null) {
                return new MoveInRangeAndShootProjectileAtActor(targetActor, projectileAttackAbility);
            }

            return new AttackUnit(targetActor);
        }
    }
}
