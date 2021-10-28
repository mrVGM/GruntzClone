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
        public struct ExecutionState
        {
            public GeneralExecutionState GeneralState;
            public AnimationExecutionState AnimationState;

            public static ExecutionState DefaultState = new ExecutionState {
                GeneralState = GeneralExecutionState.Playing,
                AnimationState = AnimationExecutionState.AnimationNotPlaying
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

            IEnumerator<ExecutionState> playAbility()
            {
                var abilityExecution = AbilityDef.Execute(ctx);
                var crt = abilityExecution.Coroutine;
                crt.MoveNext();
                while (crt.Current != AbilityDef.AbilityProgress.Finished)
                {
                    yield return crt.Current == AbilityDef.AbilityProgress.PlayingAnimation ?
                        new ExecutionState { GeneralState = GeneralExecutionState.Playing, AnimationState = AnimationExecutionState.AnimationPlaying } :
                        new ExecutionState { GeneralState = GeneralExecutionState.Playing, AnimationState = AnimationExecutionState.AnimationNotPlaying };
                    crt.MoveNext();
                }
                yield return new ExecutionState { GeneralState = GeneralExecutionState.Finished, AnimationState = AnimationExecutionState.AnimationNotPlaying };
            }

            IEnumerator<ExecutionState> playAndInterrupt()
            {
                var crt = playAbility();
                while (true) {
                    if (Interrupted) {
                        yield return new ExecutionState { GeneralState = GeneralExecutionState.Interrupted, AnimationState = AnimationExecutionState.AnimationNotPlaying };
                        break;
                    }
                    crt.MoveNext();
                    yield return crt.Current;
                    if (crt.Current.GeneralState == GeneralExecutionState.Finished) {
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
                        ctx.OnFinished();
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
