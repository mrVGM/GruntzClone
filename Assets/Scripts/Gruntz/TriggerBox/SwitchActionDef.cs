using Base.Actors;
using Base.Status;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class SwitchActionDef : TriggerBoxActionDef
    {
        public enum SwitchMode
        {
            OnStep,
            OnStay,
            OnFirstStep
        }

        public SwitchMode Mode = SwitchMode.OnStay;
        public StatusDef AlreadySwitched;

        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var otherActorProxy = otherCollider.GetComponent<ActorProxy>();
            if (otherActorProxy == null) {
                return;
            }

            var statusComponent = ownActor.GetComponent<StatusComponent>();
            if (Mode == SwitchMode.OnFirstStep && statusComponent.GetStatus(AlreadySwitched) != null) {
                return;
            }

            var switchComponent = ownActor.GetComponent<SwitchState.SwitchStateComponent>();

            IEnumerator<StatusDef> getStatesEnumerator()
            {
                int index = 0;
                while (true) {
                    yield return switchComponent.SwitchStateComponentDef.StateStatuses[index];
                    index = (index + 1) % switchComponent.SwitchStateComponentDef.StateStatuses.Length;
                }
            }

            var states = getStatesEnumerator();
            states.MoveNext();

            var currentState = switchComponent.GetCurrentState();
            while (states.Current != currentState) {
                states.MoveNext();
            }
            states.MoveNext();
            switchComponent.SetCurrentState(states.Current);

            if (Mode == SwitchMode.OnFirstStep) {
                statusComponent.AddStatus(AlreadySwitched.Data.CreateStatus());
            }
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
            if (Mode != SwitchMode.OnStay) {
                return;
            }

            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var otherActorProxy = otherCollider.GetComponent<ActorProxy>();
            if (otherActorProxy == null) {
                return;
            }

            var switchComponent = ownActor.GetComponent<SwitchState.SwitchStateComponent>();

            IEnumerator<StatusDef> getStatesEnumerator() {
                int index = 0;
                while (true) {
                    yield return switchComponent.SwitchStateComponentDef.StateStatuses[index];
                    index = (index + 1) % switchComponent.SwitchStateComponentDef.StateStatuses.Length;
                }
            }

            var states = getStatesEnumerator();
            states.MoveNext();

            var currentState = switchComponent.GetCurrentState();
            while (states.Current != currentState) {
                states.MoveNext();
            }
            states.MoveNext();
            switchComponent.SetCurrentState(states.Current);
        }
    }
}
