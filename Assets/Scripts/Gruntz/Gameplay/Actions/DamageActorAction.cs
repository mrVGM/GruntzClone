using Base.Actors;
using Base.Status;
using Gruntz.Statuses;
using System.Linq;
using UnityEngine;

namespace Gruntz.Gameplay.Actions
{
    public class DamageActorAction : IGameplayAction
    {
        public float DamageValue;
        public Actor Actor { get; set; }

        Actor IGameplayAction.Actor => throw new System.NotImplementedException();

        public void Execute()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
            var healthStatusData = healthStatus.StatusData as HealthStatusData;
            healthStatusData.Health -= DamageValue;
            healthStatusData.Health = Mathf.Max(0, healthStatusData.Health);
        }
    }
}
