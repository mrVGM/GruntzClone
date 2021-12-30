using Base;
using Base.Actors;
using Base.Gameplay;
using Base.MessagesSystem;
using Base.Navigation;
using Base.Status;
using Gruntz.Actors;
using Gruntz.Gameplay;
using Gruntz.Projectile;
using Gruntz.Statuses;
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
        public float Damage = 10;

        public ActorTemplateDef Projectile;

        public float DamageAmount => Damage;

        public override AbilityExecution Execute(AbilityExecutionContext ctx)
        {
            var actor = ctx.Actor;
            Vector3 targetPosition = (Vector3) ctx.Target;
            
            IEnumerator<ExecutionState> crt()
            {
                var projectile = ActorDeployment.DeployActorFromTemplate(Projectile, -1000 * Vector3.down);
                var projectileComponent = projectile.GetComponent<ProjectileComponent>();

                var game = Game.Instance;
                var repo = game.DefRepositoryDef;
                var actorIDStatusDef = repo.AllDefs.OfType<ActorIDStatusDef>().FirstOrDefault();
                var statusComponent = actor.GetComponent<StatusComponent>();
                var idStatus = statusComponent.GetStatus(actorIDStatusDef);
                var statusData = idStatus.StatusData as ActorIDStatusData;

                projectileComponent.Data = new ProjectileComponentData { OwnerActorId = statusData.ID, LifeTime = 0, StartPoint = actor.Pos, EndPoint = targetPosition };
                yield return new ExecutionState {
                    GeneralState = GeneralExecutionState.Finished,
                    CooldownState = CooldownState.NeedsCooldown,
                };
            }

            return new AbilityExecution {
                Coroutine = crt(),
                OnFinishedCallback = ctx.OnFinished,
            };
        }
    }
}
