using Base;
using Base.Actors;
using Base.MessagesSystem;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.UnitController
{
    public class UnitController : IOrderedUpdate, IActorComponent
    {
        public List<MessagesBoxTagDef> MessageBoxTagStack = new List<MessagesBoxTagDef>();
        public MessagesBoxTagDef MessagesBox => MessageBoxTagStack.LastOrDefault();
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
            MessageBoxTagStack.Add(unitControllerDef.MessagesBoxTagDef);
            orderTagDef = unitControllerDef.OrderTagDef;
        }
    }
}
