using Base.Actors;
using Gruntz.Actors;

namespace Gruntz.Puzzle
{
    public interface IGameplayAction
    {
        Actor Actor { get; }
        void Execute();
    }
}
