using Base.Actors;
using Base.Gameplay;
using Base.Status;

namespace Gruntz.Gameplay.Actions
{
    public class AddStatusAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Status Status;
    }
}
