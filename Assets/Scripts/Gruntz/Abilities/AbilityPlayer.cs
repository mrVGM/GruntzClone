using Base.Actors;
using Gruntz.Equipment;
using System;
using System.Collections.Generic;

namespace Gruntz.Abilities
{
    public class AbilityPlayer
    {
        public enum GeneralExecutionState
        {
            Playing,
            Finished,
            Interrupted,
        }

        public enum AnimationExecutionState
        {
            AnimationNotPlaying,
            AnimationPlaying,
        }
        public enum CooldownState
        {
            NeedsCooldown,
            NoCooldown
        }

        public struct ExecutionState
        {
            public GeneralExecutionState GeneralState;
            public AnimationExecutionState AnimationState;
            public CooldownState CooldownState;

            public static ExecutionState DefaultState = new ExecutionState {
                GeneralState = GeneralExecutionState.Playing,
                AnimationState = AnimationExecutionState.AnimationNotPlaying,
                CooldownState = CooldownState.NeedsCooldown,
            };
        }
        public AbilityDef AbilityDef { get; }
        public Actor Actor { get; }
        public object Target { get; }
        public IEnumerator<ExecutionState> Execution { get; }
        public bool Interrupted { get; private set; } = false;

        public ExecutionState State { get; private set; } = ExecutionState.DefaultState;
        public AbilityPlayer(AbilityDef abilityDef, AbilityDef.AbilityExecutionContext ctx)
        {
            AbilityDef = abilityDef;
            Actor = ctx.Actor;
            Target = ctx.Target;

            var abilityExecution = AbilityDef.Execute(ctx);
            IEnumerator<ExecutionState> playAbility()
            {
                var crt = abilityExecution.Coroutine;
                while (true) {
                    crt.MoveNext();
                    var cur = crt.Current;
                    yield return cur;
                    if (cur.GeneralState != GeneralExecutionState.Playing) {
                        break;
                    }
                }
            }

            IEnumerator<ExecutionState> playAndInterrupt()
            {
                var crt = playAbility();
                while (true) {
                    crt.MoveNext();
                    var cur = crt.Current;

                    if (Interrupted) {
                        cur.GeneralState = GeneralExecutionState.Interrupted;
                        cur.AnimationState = AnimationExecutionState.AnimationNotPlaying;
                    }

                    yield return cur;
                    if (cur.GeneralState != GeneralExecutionState.Playing) {
                        break;
                    }
                }
            }

            IEnumerator<ExecutionState> playAndFixEquipment()
            {
                var equipment = Actor.GetComponent<EquipmentComponent>();
                var crt = playAndInterrupt();

                bool animationPlaying = false;
                while (true) {
                    crt.MoveNext();
                    if (crt.Current.GeneralState != GeneralExecutionState.Playing) {
                        equipment.EnableLagging(true);
                        abilityExecution.OnFinishedCallback();
                        yield return crt.Current;
                        break;
                    }
                    if (crt.Current.AnimationState == AnimationExecutionState.AnimationPlaying && !animationPlaying) {
                        equipment.EnableLagging(false);
                        animationPlaying = true;
                    }
                    yield return crt.Current;
                }
            }

            Execution = playAndFixEquipment();
        }
        public void Update()
        {
            if (State.GeneralState == GeneralExecutionState.Playing) {
                Execution.MoveNext();
            }
            State = Execution.Current;
        }

        public void Interrupt()
        {
            Interrupted = true;
        }
    }
}
