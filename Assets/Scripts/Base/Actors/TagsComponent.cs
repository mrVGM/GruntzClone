using System.Collections.Generic;

namespace Base.Actors
{
    public class TagsComponent : IActorComponent
    {
        public TagsComponentDef TagsComponentDef { get; }
        public TagsComponent(TagsComponentDef tagsComponentDef)
        {
            TagsComponentDef = tagsComponentDef;
        }

        public IEnumerable<TagDef> Tags => TagsComponentDef.Tags;

        public void DeInit()
        {
        }

        public void Init()
        {
        }
    }
}
