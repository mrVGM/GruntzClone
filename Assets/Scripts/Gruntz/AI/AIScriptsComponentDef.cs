using Base.Actors;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIScriptsComponentDef : ActorComponentDef
    {
        public TextAsset ParserTable;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AIScriptsComponent(actor, this);
        }
    }
}
