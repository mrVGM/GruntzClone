using Base;
using Base.Status;
using System;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Gruntz.SwitchState
{
    public class AnimatorStateBehaviourDef : Def
    {
        public AnimatorController BaseController;
        public AnimationClip[] DefaultClips;
    }
}
