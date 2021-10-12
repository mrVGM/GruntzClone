using Base;
using Base.Actors;
using System.Linq;

namespace Gruntz.AI
{
    public class SimpleAIComponent : IActorComponent, IOrderedUpdate
    {
        public SimpleAIComponentDef SimpleAIComponentDef { get; }
        public Actor Actor { get; }
        private IAIAction _currentAction;

        public IAIAction CurrentAction
        {
            get => _currentAction;
            set
            {
                if (_currentAction != null) {
                    _currentAction.Stop();
                }
                _currentAction = value;
            }
        }

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var defRepository = Game.Instance.DefRepositoryDef;
                return defRepository.AllDefs.OfType<SimpleAIOrderTagDef>().FirstOrDefault();
            }
        }

        public SimpleAIComponent(SimpleAIComponentDef simpleAIComponentDef, Actor actor)
        {
            SimpleAIComponentDef = simpleAIComponentDef;
            Actor = actor;
        }
        public void DeInit()
        {
            var mainUpdater = Game.Instance.MainUpdater;
            mainUpdater.UnRegisterUpdatable(this);
        }

        public void Init()
        {
            var mainUpdater = Game.Instance.MainUpdater;
            mainUpdater.RegisterUpdatable(this);
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            if (CurrentAction != null && !CurrentAction.CanProceed()) {
                CurrentAction = null;
            }
            if (CurrentAction != null) {
                CurrentAction.Update();
            }
        }
    }
}
