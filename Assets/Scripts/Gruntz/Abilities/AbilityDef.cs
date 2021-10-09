using Base;
using Base.Actors;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class AbilityDef : Def
    {
        public AnimationClip Animation;
        public IEnumerator<object> Execute(Actor actor, object target)
        {
            float time = Time.time;
            var stopwatch = Stopwatch.StartNew();
            while (Time.time - time < 5) {
                yield return null;
            }

            yield break;
        }
    }
}
