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

        IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            foreach (var actionGenerator in _gameplayManagerDef.GameplayActionGenerators) {
                var actions = actionGenerator.GenerateActions(gameplayEvents);
                foreach (var action in actions) {
                    yield return action;
                }
            }
        }

        private void ProcessGameplayEvents(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var actions = GenerateActions(gameplayEvents);

            foreach (var action in actions) {
                action.Execute();
            }
        }

        public void DoUpdate()
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
