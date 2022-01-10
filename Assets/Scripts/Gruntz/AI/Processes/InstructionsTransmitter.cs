using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;

namespace Gruntz.AI.Processes
{
    public class InstructionsTransmitter : CoroutineProcess
    {
        public MessagesBoxTagDef MessagesBoxTagDef;
        public ProcessContextTagDef InstructionsAccumulatedDef;

        private IEnumerable<IUnitExecutable> GetAccumulatedInstructions()
        {
            List<IUnitExecutable> instructions =
                context.GetItem(InstructionsAccumulatedDef) as List<IUnitExecutable>;
            if (instructions == null) {
                yield break;
            }

            foreach (var instruction in instructions) {
                yield return instruction;
            }
        }

        private void ClearInstructions()
        {
            context.PutItem(InstructionsAccumulatedDef, null);
        }

        protected override IEnumerator<object> Crt()
        {
            int instructionPriority(IUnitExecutable instruction)
            {
                if (instruction is AttackUnit) {
                    return 1000;
                }
                return 1;
            }

            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            
            while (true)
            {
                var instructions = GetAccumulatedInstructions();
                var instruction = instructions.OrderByDescending(instructionPriority).FirstOrDefault();
                if (instruction != null) {
                    messagesSystem.SendMessage(
                        MessagesBoxTagDef,
                        MainUpdaterUpdateTime.Update,
                        this,
                        new UnitControllerInstruction {
                            Unit = possessedActor,
                            Executable = instruction,
                        });
                }
                ClearInstructions();
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            ClearInstructions();
            yield break;
        }
    }
}
