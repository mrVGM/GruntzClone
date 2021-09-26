using Base.Actors;
using Base.Status;

namespace Gruntz.Gameplay.Actions
{
    public class ActorArrivedAtArrowDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Status[] StatusesToRemove;

        public void Execute()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            foreach (var status in StatusesToRemove) {
                statusComponent.RemoveStatus(status);
            }
        }
    }
}
