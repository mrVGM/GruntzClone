using Base.Actors;
using Base.Navigation;
using Base.Status;
using UnityEngine;

namespace Gruntz.Gameplay.Actions
{
    public class RedirectActorAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status[] StatusesToAdd;

        public void Execute()
        {
            var navAgent = Actor.GetComponent<NavAgent>();
            navAgent.Target = Destination;
            var statusComponent = Actor.GetComponent<StatusComponent>();
            foreach (var status in StatusesToAdd) {
                statusComponent.AddStatus(status);
            }
        }
    }
}
