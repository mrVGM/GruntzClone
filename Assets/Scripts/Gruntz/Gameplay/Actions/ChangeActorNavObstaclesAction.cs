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
    }
}
