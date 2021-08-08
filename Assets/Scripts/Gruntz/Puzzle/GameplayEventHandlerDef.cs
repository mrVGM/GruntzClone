using System.Collections.Generic;
using Base;

namespace Gruntz.Puzzle
{
    public abstract class GameplayEventHandlerDef : Def
    {
        public abstract int GetPriority(IEnumerable<GameplayEvent> gameplayEvents);
        public abstract void HandleEvents(IEnumerable<GameplayEvent> gameplayEvents);
    }
}
