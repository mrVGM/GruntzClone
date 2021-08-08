using Base;
using System.Collections.Generic;
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

        private GameplayManagerDef _gameplayManagerDef;
        private List<GameplayEvent> _eventsCollected = new List<GameplayEvent>();

        public GameplayManager(GameplayManagerDef gameplayManagerDef)
        {
            _gameplayManagerDef = gameplayManagerDef;
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }

        public void HandleGameplayEvent(GameplayEvent gameplayEvent)
        {
            _eventsCollected.Add(gameplayEvent);
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

        private void ProcessGameplayEvents()
        {
            var handlers = _gameplayManagerDef.Handlers.Where(x => x.GetPriority(_eventsCollected) >= 0)
                            .OrderByDescending(x => x.GetPriority(_eventsCollected));

            foreach (var handler in handlers)
            {
                handler.HandleEvents(_eventsCollected);
            }
        }

        public void DoUpdate()
        {
            ProcessGameplayEvents();
            _eventsCollected.Clear();
        }
    }
}
