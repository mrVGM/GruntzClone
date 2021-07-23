using Base;
using System.Linq;

namespace Gruntz.Puzzle
{
    public class GameplayManager : IContextObject
    {
        public void HandleGameplayEvent(GameplayEvent gameplayEvent)
        {
        }

        public void DisposeObject()
        {
        }

        public static GameplayManager GetActorManagerFromContext()
        {
            var game = Game.Instance;
            var gameplayManagerDef = game.DefRepositoryDef.AllDefs.OfType<GameplayManagerDef>().First();
            var context = game.Context;
            var gameplayManager = context.GetRuntimeObject(gameplayManagerDef) as GameplayManager;
            return gameplayManager;
        }
    }
}
