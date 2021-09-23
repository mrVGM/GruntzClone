using Base;
using System.Collections.Generic;

namespace Gruntz.Puzzle
{
    public abstract class GameplayActionGenerator : Def
    {
        public abstract IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents);
    }
}
