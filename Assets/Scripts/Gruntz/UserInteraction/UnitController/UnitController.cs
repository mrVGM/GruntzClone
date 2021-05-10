using Base;
using Base.MessagesSystem;
using Gruntz.Actors;

namespace Gruntz.UserInteraction.UnitController
{
    public class UnitController : IOrderedUpdate
    {
        private MessagesBoxTagDef MessagesBox { get; }
        public Actor Unit { get; }

        private ExecutionOrderTagDef orderTagDef { get; }
        public ExecutionOrderTagDef OrderTagDef => orderTagDef;
        public void DoUpdate()
        {
        }

        public UnitController(Actor unit, UnitControllerDef unitControllerDef)
        {
            Unit = unit;
            MessagesBox = unitControllerDef.MessagesBoxTagDef;
            orderTagDef = unitControllerDef.OrderTagDef;
        }
    }
}
