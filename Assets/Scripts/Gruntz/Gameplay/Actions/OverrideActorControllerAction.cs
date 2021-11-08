using Base.Actors;
using Base.Gameplay;
using Base.MessagesSystem;

namespace Gruntz.Gameplay.Actions
{
    public class OverrideActorControllerAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public MessagesBoxTagDef MessagesBoxTagDef;
        public bool Restore = false;
    }
}
