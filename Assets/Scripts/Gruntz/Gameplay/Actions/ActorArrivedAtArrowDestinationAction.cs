using Base.Actors;
using Base.Status;

namespace Gruntz.Gameplay.Actions
{
    public class ActorArrivedAtArrowDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Status[] StatusesToRemove;
    }
}
