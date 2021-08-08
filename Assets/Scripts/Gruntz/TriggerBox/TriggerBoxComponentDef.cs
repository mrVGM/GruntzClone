using Gruntz.Actors;
using Gruntz.Status;

namespace Gruntz
{
    public class TriggerBoxComponentDef : ActorComponentDef
    {
        public StatusDef StatusDef;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new TriggerBoxComponent(this, actor);
        }
    }
}
