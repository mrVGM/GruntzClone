using Base.Actors;
using Base.MessagesSystem;

namespace Base.Animations
{
    public class AnimationsComponentDef : ActorComponentDef
    {
        public MessagesBoxTagDef NavigationMessages;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AnimationsComponent(actor, this);
        }
    }
}
