using Base.Actors;

namespace Base.Navigation
{
    public class SimplePositionComponentDef : ActorComponentDef
    {
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var simplePositionComponent = new SimplePositionComponent(this, actor);
            return simplePositionComponent;
        }
    }
}
