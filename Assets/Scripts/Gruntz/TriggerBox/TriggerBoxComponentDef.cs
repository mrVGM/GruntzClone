using Gruntz.Actors;

namespace Gruntz
{
    public class TriggerBoxComponentDef : ActorComponentDef
    {
        public TriggerStatusActionDef StatusActionDef;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new TriggerBoxComponent(this, actor);
        }
    }
}
