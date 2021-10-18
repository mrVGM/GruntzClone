using Base.Actors;
using UnityEngine;

namespace Gruntz.Highlight
{
    public class HighlightComponentDef : ActorComponentDef
    {
        public Material HighlightMaterial;

        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new HighlightComponent(actor, this);
        }
    }
}
