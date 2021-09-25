using Base.Actors;
using Gruntz.Navigation;
using Gruntz.Status;
using UnityEngine;

namespace Gruntz.Puzzle.Actions
{
    public class RedirectBallActorAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status.Status StatusToRemove;

        public void Execute()
        {
            var navAgent = Actor.GetComponent<NavAgent>();
            navAgent.Target = Destination;
            var statusComponent = Actor.GetComponent<StatusComponent>();
            statusComponent.RemoveStatus(StatusToRemove);
        }
    }
}
