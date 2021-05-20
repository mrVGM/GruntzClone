using Base;
using Gruntz.Navigation;
using Gruntz.UserInteraction.UnitController;
using UnityEngine;

namespace Gruntz.Actors
{
    public class Actor
    {
        ActorData _actorData;
        private UnitController unitController;
        public ActorComponent ActorComponent { get; private set; }
        public UnitController UnitController
        {
            get
            {
                if (unitController == null)
                {
                    unitController = new UnitController(this, _actorData.UnitControllerDef);
                    var game = Game.Instance;
                    game.MainUpdater.RegisterUpdatable(unitController);
                }
                return unitController;
            }
        }

        public Vector3 Pos => ActorComponent.GetComponent<NavAgent>().ActorVisuals.position;

        public Actor(ActorComponent actorComponent, ActorData actorData)
        {
            _actorData = actorData;
            ActorComponent = actorComponent;
            ActorComponent.ActorData = _actorData;
            var game = Game.Instance;

            var navAgent = ActorComponent.GetComponent<NavAgent>();
            game.MainUpdater.RegisterUpdatable(navAgent);
            var controller = UnitController;
        }
        public void Deinit()
        {
            var game = Game.Instance;
            var navAgent = ActorComponent.GetComponent<NavAgent>();
            game.MainUpdater.UnRegisterUpdatable(navAgent);
        }
    }
}
