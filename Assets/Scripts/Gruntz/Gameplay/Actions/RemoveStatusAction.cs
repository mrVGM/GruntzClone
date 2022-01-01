using Base.Actors;
using Base.Gameplay;
using Base.Status;

namespace Gruntz.Gameplay.Actions
{
    public class RemoveStatusAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Status Status;
    }
}
