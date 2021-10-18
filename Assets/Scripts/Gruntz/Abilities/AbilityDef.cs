using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.Status;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Abilities
{
    public abstract class AbilityDef : Def
    {
        public float Cooldown = 0;
        public AnimationClip Animation;
        public MessagesBoxTagDef AnimationEventMessages;
        public StatusDef[] TargetActorStatuses;
        public abstract IEnumerator<object> Execute(Actor actor, object target);
    }
}
