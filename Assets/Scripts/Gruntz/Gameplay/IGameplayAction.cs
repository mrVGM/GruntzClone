using Base.Actors;

namespace Gruntz.Gameplay
{
    public interface IGameplayAction
    {
        Actor Actor { get; }
    }
}
