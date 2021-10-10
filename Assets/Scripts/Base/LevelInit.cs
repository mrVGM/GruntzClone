using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Base.MainUpdaterLock;

namespace Base
{
    public class LevelInit : MonoBehaviour, IOrderedUpdate
    {
        public UnityEvent OnLevelLoaded;

        public ExecutionOrderTagDef OrderTagDef 
        {
            get
            {
                var game = Game.Instance;
                return game.DefRepositoryDef.AllDefs.OfType<LevelLoadedOrderTagDef>().FirstOrDefault();
            }
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            var game = Game.Instance;
            game.MainUpdater.UnRegisterUpdatable(this);
            OnLevelLoaded.Invoke();
        }

        public void InitLevel() 
        {
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }
    }
}
