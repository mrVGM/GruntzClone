using Base.Actors;
using Gruntz.Equipment;
using System;
using System.Collections.Generic;

namespace Gruntz.Abilities
{
    public class AbilityPlayer
    {
        public enum ExecutionState
        {
            Playing,
            AnimationPlaying,
            Finished,
            Interrupted,
        }
        public AbilityDef AbilityDef { get; }
        public Actor Actor { get; }
        public object Target { get; }
        public IEnumerator<ExecutionState> Execution { get; }
        public bool Interrupted { get; private set; } = false;

        public ExecutionState State { get; private set; } = AbilityPlayer.ExecutionState.Playing;
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
                    yield return crt.Current == AbilityDef.AbilityProgress.PlayingAnimation ? ExecutionState.AnimationPlaying : ExecutionState.Playing;
                    crt.MoveNext();
                }
                yield return ExecutionState.Finished;
            }

            IEnumerator<ExecutionState> playAndInterrupt()
            {
                var crt = playAbility();
                while (true) {
                    if (Interrupted) {
                        yield return ExecutionState.Interrupted;
                        break;
                    }
                    crt.MoveNext();
                    yield return crt.Current;
                    if (crt.Current == ExecutionState.Finished) {
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
                    if (crt.Current == ExecutionState.Finished || crt.Current != ExecutionState.Interrupted) {
                        equipment.EnableLagging(true);
                        ctx.OnFinished();
                        yield return crt.Current;
                        break;
                    }
                    if (crt.Current == ExecutionState.AnimationPlaying && !animationPlaying) {
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
            if (State == ExecutionState.Playing) {
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
