using Base.MessagesSystem;
using Gruntz.Actors;
using Gruntz.Navigation;
using Gruntz.Puzzle.Statuses;
using Gruntz.UserInteraction.UnitController;

namespace Gruntz.Puzzle.Actions
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
