using Base.Actors;
using Base.Status;
using System.Linq;

namespace Gruntz.SwitchState
{
    public class SwitchStateComponent : IActorComponent
    {
        public SwitchStateComponentDef SwitchStateComponentDef { get; }
        public Actor Actor { get; }
        public SwitchStateComponent(SwitchStateComponentDef switchStateComponentDef, Actor actor)
        {
            SwitchStateComponentDef = switchStateComponentDef;
            Actor = actor;
        }
        public void DeInit()
        {
        }

        public void Init()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            var state = statusComponent
                .GetStatuses(x => SwitchStateComponentDef.StateStatuses.Contains(x.StatusDef))
                .FirstOrDefault();
            if (state != null) {
                SetCurrentState(state.StatusDef);
            }
        }

        public void SetCurrentState(StatusDef stateStatusDef)
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            foreach (var statusDef in SwitchStateComponentDef.StateStatuses) {
                var status = statusComponent.GetStatus(statusDef);
                if (status != null) {
                    statusComponent.RemoveStatus(status);
                }
            }
            var newStatus = stateStatusDef.Data.CreateStatus();
            statusComponent.AddStatus(newStatus);
            UpdateState();
        }

        private void UpdateState()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            var status = statusComponent
                .GetStatuses(x => SwitchStateComponentDef.StateStatuses.Contains(x.StatusDef))
                .FirstOrDefault();
            if (status == null) {
                return;
            }
            var switchStateBehaviour = Actor.ActorComponent.GetComponentInChildren<ISwitchStateBehaviour>();
            switchStateBehaviour.SwitchState(status.StatusDef);
        }
    }
}
