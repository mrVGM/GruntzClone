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
    }
}
