namespace Base.Actors
{
    public class TagsComponentDef : ActorComponentDef
    {
        public TagDef[] Tags;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new TagsComponent(this);
        }
    }
}
