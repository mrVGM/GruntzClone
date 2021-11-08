using Base;
using System.Collections.Generic;

namespace Base.Gameplay
{
    public abstract class GameplayActionGenerator : Def
    {
        public abstract IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents);
    }
}
