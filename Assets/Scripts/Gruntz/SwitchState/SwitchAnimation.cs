using Base.Actors;
using Base.Status;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.SwitchState
{
    public class SwitchAnimation : MonoBehaviour
    {
        public int State;
        private int _stateChached = -1;

        public ActorProxy ActorProxy;

        private void Update()
        {
            if (_stateChached == State) {
                return;
            }

            var actor = ActorProxy.Actor;
            if (actor == null) {
                return;
            }

            _stateChached = State;
            var switchComponent = actor.GetComponent<SwitchState.SwitchStateComponent>();

            int index = State % switchComponent.SwitchStateComponentDef.StateStatuses.Length;
            var cur = switchComponent.SwitchStateComponentDef.StateStatuses[index];
            switchComponent.SetCurrentState(cur);
        }
    }
}
