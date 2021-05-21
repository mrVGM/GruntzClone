using Base;
using Base.MessagesSystem;
using Gruntz.Actors;
using System.Linq;

namespace Gruntz.UserInteraction.UnitController
{
    public class UnitController : IOrderedUpdate, IActorComponent
    {
        private MessagesBoxTagDef MessagesBox { get; }
        public Actor Unit { get; }

        private ExecutionOrderTagDef orderTagDef { get; }
        public ExecutionOrderTagDef OrderTagDef => orderTagDef;
        public void DoUpdate()
        {
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            var messages = messagesSystem.GetMessages(MessagesBox).Select(x => (UnitControllerInstruction)x.Data);
            var instructionForMe = messages.Where(x => x.Unit == Unit);
            foreach (var instruction in instructionForMe)
            {
                instruction.Executable.Execute(Unit);
            }
        }

        public void Init()
        {
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }

        public void DeInit()
        {
            var game = Game.Instance;
            game.MainUpdater.UnRegisterUpdatable(this);
        }

        public UnitController(Actor unit, UnitControllerDef unitControllerDef)
        {
            Unit = unit;
            MessagesBox = unitControllerDef.MessagesBoxTagDef;
            orderTagDef = unitControllerDef.OrderTagDef;
        }
    }
}
