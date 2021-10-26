using Base.Actors;
using Base.Navigation;
using Base.Status;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Gameplay.Actions
{
    public class SetInitialDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status StatusToRemove;
    }
}
