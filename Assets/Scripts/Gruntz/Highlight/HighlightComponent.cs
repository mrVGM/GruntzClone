using Base.Actors;
using System.Linq;

namespace Gruntz.Highlight
{
    public class HighlightComponent : IActorComponent
    {
        public Actor Actor { get; }
        public HighlightComponentDef HighlightComponentDef { get; }
        public HighlightComponent(Actor actor, HighlightComponentDef highlightComponentDef)
        {
            Actor = actor;
            HighlightComponentDef = highlightComponentDef;
        }
        public void DeInit()
        {
        }

        public void Init()
        {
        }

        public void SetHighlight(bool active)
        {
            var highlightBehaviours = Actor.ActorComponent.GetComponentsInChildren<HighlightBehaviour>();
            foreach (var behaviour in highlightBehaviours) {
                foreach (var rend in behaviour.Renderers) {
                    if (active && rend.materials.Length == 1) {
                        rend.materials = rend.materials.Append(HighlightComponentDef.HighlightMaterial).ToArray();
                    }
                    if (!active && rend.materials.Length > 1) {
                        rend.materials = rend.materials.Take(1).ToArray();
                    }
                }
            }
        }
    }
}
