using Base.Actors;
using Base.Gameplay;
using Base.Status;
using UnityEngine;

namespace Gruntz.Gameplay.Actions
{
    public class SetInitialDestinationAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Vector3 Destination;
        public Status StatusToRemove;
    }
}
