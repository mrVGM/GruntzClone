using System.Collections.Generic;

namespace Gruntz.Puzzle.Gameplay
{
    public interface IFilterNode
    {
        bool Filter(IEnumerable<GameplayEvent> gameplayEvents);
    }
}
