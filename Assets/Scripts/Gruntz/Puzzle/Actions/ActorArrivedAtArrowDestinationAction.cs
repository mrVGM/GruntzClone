using Base.Actors;
using Gruntz.Actors;
using Gruntz.Status;

namespace Gruntz.Puzzle.Actions
{
    public class ActorArrivedAtArrowDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Status.Status[] StatusesToRemove;

        public void Execute()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            foreach (var status in StatusesToRemove) {
                statusComponent.RemoveStatus(status);
            }
        }
    }
}
