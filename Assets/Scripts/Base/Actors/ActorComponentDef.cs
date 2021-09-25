namespace Base.Actors
{
    public abstract class ActorComponentDef : Def
    {
        public abstract IActorComponent CreateActorComponent(Actor actor);
    }
}
