using Base;
using System.Collections.Generic;
using System.Linq;

namespace Base.Gameplay
{
    public class GameplayManager : IContextObject, IOrderedUpdate
    {
        private List<GameplayActionGenerator> __actionGenerators;

        private IEnumerable<GameplayActionGenerator> _actionGenerators
        {
            get
            {
                if (__actionGenerators == null) {
                    var game = Game.Instance;
                    var repo = game.DefRepositoryDef;
                    __actionGenerators = repo.AllDefs.OfType<GameplayActionGenerator>().ToList();
                }
                return __actionGenerators;
            }
        }

        public ExecutionOrderTagDef OrderTagDef => _gameplayManagerDef.GameplayManagerExecutionOrderTag;

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

        public static GameplayManager GetGameplayManagerFromContext()
        {
            var game = Game.Instance;
            var gameplayManagerDef = game.DefRepositoryDef.AllDefs.OfType<GameplayManagerDef>().First();
            var context = game.Context;
            var gameplayManager = context.GetRuntimeObject(gameplayManagerDef) as GameplayManager;
            return gameplayManager;
        }

        IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            foreach (var actionGenerator in _actionGenerators) {
                var actions = actionGenerator.GenerateActions(gameplayEvents);
                foreach (var action in actions) {
                    yield return action;
                }
            }
        }

        private void ProcessGameplayEvents(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var actions = GenerateActions(gameplayEvents);
            _gameplayManagerDef.gameplayActionsProcessorDef.ProcessActions(actions);
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            while (_eventsCollected.Any())
            {
                var eventsCache = _eventsCollected.ToList();
                _eventsCollected.Clear();
                ProcessGameplayEvents(eventsCache);
            }
        }
    }
}
