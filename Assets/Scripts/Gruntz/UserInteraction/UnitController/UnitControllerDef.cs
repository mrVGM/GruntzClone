using Base;
using Base.Actors;
using Base.MessagesSystem;
using Gruntz.Actors;

namespace Gruntz.UserInteraction.UnitController
{
    public class UnitControllerDef : ActorComponentDef
    {
        public MessagesBoxTagDef MessagesBoxTagDef;
        public ExecutionOrderTagDef OrderTagDef;

        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var unitController = new UnitController(actor, this);
            return unitController;
        }
    }
}
