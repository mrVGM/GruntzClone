using Base.Actors;
using Gruntz.Actors;
using Gruntz.Puzzle.Statuses;
using Gruntz.Status;
using System.Linq;
using UnityEngine;

namespace Gruntz.Puzzle.Actions
{
    public class KillActorAction : IGameplayAction
    {
        public ActorDeployDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public Actor Actor { get; set; }

        public void Execute()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
            if (healthStatus != null)
            {
                var healthStatusData = healthStatus.StatusData as HealthStatusData;
                healthStatusData.Health = 0;
            }

            Vector3 pos = Actor.Pos;
            var deadActorData = Actor.Data as ActorData;
            Actor.Deinit();

            var actorComponents = GraveDef.ActorComponents
                    .Select(x => new ActorData.Components { _component = x.ToDefRef<ActorComponentDef>() });
            
            var graveActorData = new ActorData
            {
                ActorDef = GraveDef.ActorDef.ToDefRef<ActorDef>(),
                ActorComponents = actorComponents.ToArray()
            };
            var graveActor = ActorDeployment.DeployActor(graveActorData);
            graveActor.ActorComponent.transform.position = pos;
            statusComponent = graveActor.GetComponent<StatusComponent>();
            var actorInstanceStatusData = ActorHolderStatusDef.Data as ActorInstanceHolderStatusData;
            actorInstanceStatusData.ActorData = deadActorData;
            statusComponent.AddStatus(actorInstanceStatusData.CreateStatus());
        }
    }
}
