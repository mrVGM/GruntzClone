using Base.Actors;
using Base.MessagesSystem;
using Gruntz.UnitController;

namespace Gruntz.Gameplay.Actions
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
                unitController.RemoveMessagesBoxTag(MessagesBoxTagDef);
            }
            else {
                unitController.PushMessagesBoxTag(MessagesBoxTagDef);
            }
        }
    }
}
