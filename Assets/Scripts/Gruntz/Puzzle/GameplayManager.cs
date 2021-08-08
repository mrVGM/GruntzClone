using Base;
using System.Linq;

namespace Gruntz.Puzzle
{
    public class GameplayManager : IContextObject, IOrderedUpdate
    {
        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var game = Game.Instance;
                var defRepo = game.DefRepositoryDef;
                return defRepo.AllDefs.OfType<GameplayManagerExecutionOrderTag>().FirstOrDefault();
            }
        }

        public GameplayManager()
        {
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }

        public void HandleGameplayEvent(GameplayEvent gameplayEvent)
        {
        }

        public void DisposeObject()
        {
            var game = Game.Instance;
            game.MainUpdater.UnRegisterUpdatable(this);
        }

        public static GameplayManager GetActorManagerFromContext()
        {
            var game = Game.Instance;
            var gameplayManagerDef = game.DefRepositoryDef.AllDefs.OfType<GameplayManagerDef>().First();
            var context = game.Context;
            var gameplayManager = context.GetRuntimeObject(gameplayManagerDef) as GameplayManager;
            return gameplayManager;
        }

        public void DoUpdate()
        {

        }
    }
}
