using Base.Actors;
using Base.Status;
using Gruntz.Statuses;
using System.Linq;
using UnityEngine;

namespace Gruntz.Gameplay.Actions
{
    public class DamageActorAction : IGameplayAction
    {
        public float DamageValue;
        public Actor Actor { get; set; }
    }
}
