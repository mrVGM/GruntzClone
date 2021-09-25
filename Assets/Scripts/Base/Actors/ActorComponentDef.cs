using Base;
using Gruntz.Actors;

namespace Base.Actors
{
    public abstract class ActorComponentDef : Def
    {
        public abstract IActorComponent CreateActorComponent(Actor actor);
    }
}
