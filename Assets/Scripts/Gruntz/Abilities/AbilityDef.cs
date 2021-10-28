using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.Status;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Gruntz.Abilities.AbilityPlayer;

namespace Gruntz.Abilities
{
    public abstract class AbilityDef : Def
    {
        public enum AbilityProgress
        {
            InProgress,
            PlayingAnimation,
            Finished
        }
        public class AbilityExecutionContext
        {
            public Actor Actor;
            public object Target;
            public Action OnFinished;
        }

        public class AbilityExecution
        {
            public IEnumerator<ExecutionState> Coroutine;
            public Action OnFinishedCallback;
        }

        public float Cooldown = 0;
        public AnimationClip Animation;
        public MessagesBoxTagDef AnimationEventMessages;
        public StatusDef[] TargetActorStatuses;
        public abstract AbilityExecution Execute(AbilityExecutionContext ctx);
    }
}
