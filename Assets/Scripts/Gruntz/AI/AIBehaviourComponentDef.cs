using Base.Actors;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIBehaviourComponentDef : ActorComponentDef
    {
        public TextAsset ParserTable;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AIBehaviourComponent(actor, this);
        }
    }
}
