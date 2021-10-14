using Base.Actors;
using Gruntz.Actors;
using Base.Status;
using System.Linq;
using UnityEngine;
using Gruntz.Statuses;

namespace Gruntz.Gameplay.Actions
{
    public class KillActorAction : IGameplayAction
    {
        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public Actor Actor { get; set; }

        public void Execute()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            if (statusComponent != null) {
                var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
                if (healthStatus != null) {
                    var healthStatusData = healthStatus.StatusData as HealthStatusData;
                    healthStatusData.Health = 0;
                }
            }

            Vector3 pos = Actor.Pos;
            var deadActorData = Actor.Data as ActorData;
            Actor.Deinit();
            var graveActor = ActorDeployment.DeployActorFromTemplate(GraveDef, pos);
            GraveDef.ProcessActor(graveActor);
            statusComponent = graveActor.GetComponent<StatusComponent>();
            var actorInstanceStatusData = ActorHolderStatusDef.Data as ActorInstanceHolderStatusData;
            actorInstanceStatusData.ActorData = deadActorData;
            statusComponent.AddStatus(actorInstanceStatusData.CreateStatus());
        }
    }
}
