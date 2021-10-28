using Base.Actors;
using Base.MessagesSystem;
using Gruntz.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.ConflictManager.ConflictManager;

namespace Gruntz.Abilities
{
    public class InstantAttackAbilityDef : AbilityDef, IAttackAbility
    {
        public float Damage = 10;
        public float DamageAmount => Damage;

        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            var targetActor = ctx.Target as Actor;
            var conflictManager = ConflictManager.ConflictManager.GetConflictManagerFromContext();
            ILock l = null;
            IEnumerator<AbilityProgress> crt()
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
                    l = conflictManager.TryGetLock(actor, targetActor);
                    yield return AbilityProgress.InProgress;
                }

                while (true)
                {
                    if (eventsForMe().Any(x => x.stringParameter == "ActionEnd"))
                    {
                        yield return AbilityProgress.Finished;
                        yield break;
                    }

                    if (eventsForMe().Any(x => x.stringParameter == "Hit"))
                    {
                        var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                        gameplayManager.HandleGameplayEvent(new DamageActorGameplayEvent
                        {
                            Ability = this,
                            SourceActor = actor,
                            TargetActor = targetActor
                        });
                        conflictManager.ReturnLock(l);
                    }
                    yield return AbilityProgress.PlayingAnimation;
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
