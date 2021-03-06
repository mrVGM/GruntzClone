using Base.Actors;
using Base.Gameplay;
using Gruntz.Abilities;

namespace Gruntz.Gameplay
{
    public class DamageActorGameplayEvent : GameplayEvent
    {
        public IAttackAbility Ability;
        public Actor SourceActor;
        public Actor TargetActor;
    }
}
