using Base.Actors;
using Base.UI;
using Gruntz.Statuses;

namespace Gruntz.AI
{
    public class AIBehaviourComponentDef : ActorComponentDef
    {
        public BrainDef BrainDef;
        public ProcessContextTagDef AIActor;
        public ProcessContextTagDef PosessedActor;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AIBehaviourComponent(actor, this);
        }
    }
}
