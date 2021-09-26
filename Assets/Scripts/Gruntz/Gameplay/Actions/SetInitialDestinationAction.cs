using Base.Actors;
using Base.Navigation;
using Base.Status;
using UnityEngine;

namespace Gruntz.Gameplay.Actions
{
    public class SetInitialDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status StatusToRemove;

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
