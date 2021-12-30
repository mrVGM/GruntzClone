using Base.Actors;
using Base.Gameplay;
using Base.MessagesSystem;
using Base.Navigation;
using Gruntz.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Abilities.AbilityPlayer;
using static Gruntz.ConflictManager.ConflictManager;

namespace Gruntz.Abilities
{
    public class ProjectileAttackAbilityDef : AbilityDef, IAttackAbility
    {
        [Serializable]
        public class Parabola
        {
            public float Height = 2.0f;
            public int NumberOfPoints = 30;
            public float MinDist = 1.0f;
            public float MaxDist = 7.0f;
            public IEnumerable<Vector3> GetParabolaPoints(Vector3 startPoint, Vector3 endPoint)
            {
                float d = (endPoint - startPoint).magnitude;
                float c = -4.0f * Height / (d * d);

                Vector3 x = (endPoint - startPoint).normalized;
                Vector3 y = Vector3.up;

                for (int i = 0; i <= NumberOfPoints; ++i)
                {
                    float coef = (float)i / NumberOfPoints;
                    float cur = coef * d;
                    float h = c * (cur * cur - d * cur + d * d / 4.0f) + Height;

                    yield return startPoint + cur * x + h * y;
                }
            }
        }

        public float Damage = 10;

        public Parabola ParabolaSettings;
        public float DamageAmount => Damage;

        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            var targetActor = ctx.Target as Actor;
            var conflictManager = ConflictManager.ConflictManager.GetConflictManagerFromContext();
            ILock l = null;

            var navigation = Navigation.GetNavigationFromContext();
            var map = navigation.Map;

            bool isTargetStillValid()
            {
                if (!targetActor.IsInPlay) {
                    return false;
                }

                var snappedTargetPos = map.SnapPosition(targetActor.Pos);
                var neighbours = map.GetNeighbours(snappedTargetPos);

                bool isInRange = neighbours.Any(x => (x - actor.Pos).sqrMagnitude < 0.01f);
                if (!isInRange) {
                    return false;
                }

                return true;
            }

            IEnumerator<ExecutionState> crt()
            {
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                IEnumerable<AnimationEvent> eventsForMe()
                {
                    var messages = messagesSystem.GetMessages(AnimationEventMessages);
                    messages = messages.ToList();
                    messages = messages.Where(x => x.Sender == actor);
                    return messages.Select(x => x.Data as AnimationEvent);
                }

                while (l == null)
                {
                    if (!isTargetStillValid()) {
                        yield return new ExecutionState {
                            GeneralState = GeneralExecutionState.Finished,
                            AnimationState = AnimationExecutionState.AnimationNotPlaying,
                            CooldownState = CooldownState.NoCooldown
                        };
                        yield break;
                    }

                    l = conflictManager.TryGetLock(actor, targetActor);
                    yield return new ExecutionState {
                        GeneralState = GeneralExecutionState.Playing,
                        AnimationState = AnimationExecutionState.AnimationNotPlaying,
                        CooldownState = CooldownState.NoCooldown,
                    };
                }

                while (true)
                {
                    if (eventsForMe().Any(x => x.stringParameter == "ActionEnd")) {
                        yield return new ExecutionState {
                            GeneralState = GeneralExecutionState.Finished,
                            AnimationState = AnimationExecutionState.AnimationNotPlaying,
                        };
                        yield break;
                    }

                    if (eventsForMe().Any(x => x.stringParameter == "Hit")) {
                        var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                        gameplayManager.HandleGameplayEvent(new DamageActorGameplayEvent {
                            Ability = this,
                            SourceActor = actor,
                            TargetActor = targetActor
                        });
                        conflictManager.ReturnLock(l);
                    }
                    yield return new ExecutionState {
                        GeneralState = GeneralExecutionState.Playing,
                        AnimationState = AnimationExecutionState.AnimationPlaying,
                    };
                }
            }

            return new AbilityExecution {
                Coroutine = crt(),
                OnFinishedCallback = () => {
                    ctx.OnFinished();
                    if (l != null) {
                        conflictManager.ReturnLock(l);
                    }
                }
            };
        }
    }
}
