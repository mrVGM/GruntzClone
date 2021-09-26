using Base.Actors;
using Base.Navigation;
using Gruntz.Statuses;

namespace Gruntz.Gameplay.Actions
{
    public class ChangeActorNavObstaclesAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public DisableNavObstaclesStatusDef DisableNavObstaclesStatus;
        public bool Disable = false;

        public void Execute()
        {
            var navAgent = Actor.GetComponent<NavAgent>();
            var navAgentData = navAgent.Data as NavAgentData;
            if (Disable) {
                navAgentData.DisabledObstacleLayers.AddRange(DisableNavObstaclesStatus.DisabledObstacleLayers);
            }
            else {
                foreach (string layer in DisableNavObstaclesStatus.DisabledObstacleLayers) {
                    navAgentData.DisabledObstacleLayers.Remove(layer);
                }
            }
        }
    }
}
