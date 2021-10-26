using Base;
using Base.Actors;
using Base.MessagesSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.UnitController
{
    public class UnitController : IOrderedUpdate, IActorComponent
    {
        public UnitControllerDef UnitOntrollerDef;
        private List<MessagesBoxTagDef> MessageBoxTagStack = new List<MessagesBoxTagDef>();
        public MessagesBoxTagDef MessagesBox => MessageBoxTagStack.LastOrDefault();
        public Actor Unit { get; }

        private ExecutionOrderTagDef orderTagDef { get; }
        public ExecutionOrderTagDef OrderTagDef => orderTagDef;

        private IUpdatingExecutable _updatingExecutable;
        public UnitControllerState UnitControllerState { get; } = new UnitControllerState();

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            var messages = messagesSystem.GetMessages(MessagesBox).Select(x => (UnitControllerInstruction)x.Data);
            var instructionForMe = messages.Where(x => x.Unit == Unit);
            int instructionsCount = instructionForMe.Count();
            if (instructionsCount > 1) {
                Debug.LogError($"More than 1 instructions for {Unit.ActorComponent}");
            }
            if (instructionsCount > 0) {
                var instruction = instructionForMe.First();
                if (_updatingExecutable != null) {
                    _updatingExecutable.StopExecution();
                }
                _updatingExecutable = instruction.Executable.Execute(Unit);
            }

            if (_updatingExecutable != null) {
                if (!_updatingExecutable.UpdateExecutable()) {
                    _updatingExecutable = null;
                }
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
            UnitOntrollerDef = unitControllerDef;
            Unit = unit;
            MessageBoxTagStack.Add(unitControllerDef.MessagesBoxTagDef);
            orderTagDef = unitControllerDef.OrderTagDef;
        }

        public void PushMessagesBoxTag(MessagesBoxTagDef messagesBoxTagDef)
        {
            CancelInstructions();
            MessageBoxTagStack.Add(messagesBoxTagDef);
        }
        public void RemoveMessagesBoxTag(MessagesBoxTagDef messagesBoxTagDef)
        {
            CancelInstructions();
            MessageBoxTagStack.Remove(messagesBoxTagDef);
        }

        private void CancelInstructions()
        {
            if (_updatingExecutable != null) {
                _updatingExecutable.StopExecution();
            }
        }
    }
}
