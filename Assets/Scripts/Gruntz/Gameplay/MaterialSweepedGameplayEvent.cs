using Base.Actors;
using Base.Gameplay;
using Gruntz.Abilities;

namespace Gruntz.Gameplay
{
    public class MaterialSweepedGameplayEvent : GameplayEvent
    {
        public AbilityDef Ability;
        public Actor SourceActor;
        public Actor TargetActor;
    }
}
