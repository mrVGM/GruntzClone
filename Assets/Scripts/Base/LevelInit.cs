using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

        public void DoUpdate()
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
