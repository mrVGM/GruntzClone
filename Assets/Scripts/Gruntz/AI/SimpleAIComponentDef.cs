using Base.Actors;

namespace Gruntz.AI
{
    public class SimpleAIComponentDef : ActorComponentDef
    {
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new SimpleAIComponent(this, actor);
        }
    }
}
