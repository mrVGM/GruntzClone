using Base.Actors;
using Base.MessagesSystem;
using Gruntz.UnitController;

namespace Gruntz.Puzzle.Actions
{
    public class OverrideActorControllerAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public MessagesBoxTagDef MessagesBoxTagDef;
        public bool Restore = false;

        public void Execute()
        {
            var unitController = Actor.GetComponent<UnitController.UnitController>();
            if (Restore) {
                unitController.MessageBoxTagStack.Remove(MessagesBoxTagDef);
            }
            else {
                unitController.MessageBoxTagStack.Add(MessagesBoxTagDef);
            }
        }
    }
}
