using Base.Actors;
using Gruntz.Actors;
using Gruntz.Navigation;
using Gruntz.Status;
using Gruntz.UserInteraction.UnitController;
using System.Linq;

namespace Gruntz.Puzzle.Actions
{
    public class KillActorAction : IGameplayAction
    {
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

            var navAgent = Actor.GetComponent<NavAgent>();
            if (navAgent != null)
            {
                navAgent.DeInit();
            }

            var unitController = Actor.GetComponent<UnitController>();
            if (unitController != null)
            {
                unitController.DeInit();
            }

            var triggerBox = Actor.GetComponent<TriggerBoxComponent>();
            if (triggerBox != null)
            {
                triggerBox.DeInit();
            }
        }
    }
}
