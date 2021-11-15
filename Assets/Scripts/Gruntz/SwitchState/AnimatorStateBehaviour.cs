using Base;
using Base.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
            public AnimationClip Clip;
        }
        public AnimatorStateBehaviourDef Def;
        public Animator Animator;
        public StateInfo[] StatusMaterialMap;

        private int _state = 0;
        private bool _dirty = false;

        void ISwitchStateBehaviour.SwitchState(StatusDef statusDef)
        {
            if (Animator.runtimeAnimatorController == null) {
                var game = Game.Instance;
                var controller = new AnimatorOverrideController(Def.BaseController);
                Animator.runtimeAnimatorController = controller;

                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                for (int i = 0; i < StatusMaterialMap.Length; ++i) {
                    overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(Def.DefaultClips[i], StatusMaterialMap[i].Clip));
                }
                controller.ApplyOverrides(overrides);                
            }

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
