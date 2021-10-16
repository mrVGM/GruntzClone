using Base.Actors;
using Base.Navigation;
using Base.Status;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Gameplay.Actions
{
    public class RedirectBallActorAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status StatusToRemove;

        public void Execute()
        {
            var navAgent = Actor.GetComponent<NavAgent>();
            navAgent.Target = new SimpleNavTarget { Target = Destination };
            var statusComponent = Actor.GetComponent<StatusComponent>();
            statusComponent.RemoveStatus(StatusToRemove);
        }
    }
}
