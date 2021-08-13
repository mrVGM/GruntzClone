using Base;
using Gruntz.Puzzle.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

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

        private void ProcessGameplayEvents(IEnumerable<GameplayEvent> gameplayEvents)
        {
            IEnumerable<IActionNode> findActionNodes(Transform root)
            {
                var actionNode = root.GetComponent<IActionNode>();
                if (actionNode != null) {
                    yield return actionNode;
                }
                var filterNode = root.GetComponent<IFilterNode>();
                if (filterNode != null && filterNode.Filter(gameplayEvents))
                {
                    int childCount = root.childCount;
                    for (int i = 0; i < childCount; ++i)
                    {
                        var curChild = root.GetChild(i);
                        var res = findActionNodes(curChild);
                        foreach (var node in res)
                        {
                            yield return node;
                        }
                    }
                }
            }

            var actionNodes = findActionNodes(_gameplayManagerDef.DecisionTree);
            foreach (var node in actionNodes)
            {
                node.ExecuteAction(gameplayEvents);
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
