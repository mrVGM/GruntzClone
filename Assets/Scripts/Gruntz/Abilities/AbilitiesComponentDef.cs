using Base.Actors;

namespace Gruntz.Abilities
{
    public class AbilitiesComponentDef : ActorComponentDef
    {
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AbilitiesComponent(actor, this);
        }
    }
}
