namespace Gruntz.Abilities
{
    public class InstantAttackAbilityDef : InstantAbilityDef, IAttackAbility
    {
        public float Damage = 10;
        public float DamageAmount => Damage;
    }
}
