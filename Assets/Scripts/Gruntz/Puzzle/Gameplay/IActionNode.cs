using System.Collections.Generic;

namespace Gruntz.Puzzle.Gameplay
{
    public interface IActionNode
    {
        void ExecuteAction(IEnumerable<GameplayEvent> gameplayEvents);
    }
}
