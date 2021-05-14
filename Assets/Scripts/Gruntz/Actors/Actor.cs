using Base;
using Gruntz.Navigation;
using Gruntz.UserInteraction.UnitController;
using UnityEngine;

namespace Gruntz.Actors
{
    public class Actor : MonoBehaviour
    {
        public UnitControllerDef UnitControllerDef;
        private UnitController unitController;
        public UnitController UnitController
        {
            get
            {
                if (unitController == null)
                {
                    unitController = new UnitController(this, UnitControllerDef);
                    var game = Game.Instance;
                    game.MainUpdater.RegisterUpdatable(unitController);
                }
                return unitController;
            }
        }
        public NavAgent NavAgent => GetComponent<NavAgent>();
        
        public void Init()
        {
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(NavAgent);
            var controller = UnitController;
        }
        public void Deinit()
        {
            var game = Game.Instance;
            game.MainUpdater.UnRegisterUpdatable(NavAgent);
        }
    }
}
