using Base.Actors;
using Gruntz.Actors;
using Gruntz.Navigation;
using Gruntz.Status;
using UnityEngine;

namespace Gruntz.Puzzle.Actions
{
    public class SetInitialDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status.Status StatusToRemove;

        public void Execute()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            if (statusComponent.GetStatus(StatusToRemove.StatusDef) == null) {
                return;            }
            var navAgent = Actor.GetComponent<NavAgent>();
            navAgent.Target = Destination;
            statusComponent.RemoveStatus(StatusToRemove);
        }
    }
}
