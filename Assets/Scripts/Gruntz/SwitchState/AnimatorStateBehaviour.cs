using Base.Status;
using System;
using System.Linq;
using UnityEngine;

namespace Gruntz.SwitchState
{
    public class AnimatorStateBehaviour : MonoBehaviour, ISwitchStateBehaviour
    {
        [Serializable]
        public class StateInfo
        {
            public StatusDef Status;
            public int State;
        }
        public Animator Animator;
        public StateInfo[] StatusMaterialMap;

        private int _state = 0;
        private bool _dirty = false;

        void ISwitchStateBehaviour.SwitchState(StatusDef statusDef)
        {
            var pair = StatusMaterialMap.FirstOrDefault(x => x.Status == statusDef);
            _state = pair.State;
            _dirty = true;
        }
        private void LateUpdate()
        {
            if (_dirty) {
                Animator.SetInteger("State", _state);
            }
        }
    }
}
