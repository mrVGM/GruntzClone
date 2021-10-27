using Base.Actors;
using Base.MessagesSystem;
using Base.Status;

namespace Gruntz.AI
{
    public class AIComponentDef : ActorComponentDef
    {
        public StatusDef RegularActorStatusDef;
        public MessagesBoxTagDef MessagesBox;
        public float UpdateInterval = 0.5f;
        public float Range;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AIComponent(this, actor);
        }
    }
}
