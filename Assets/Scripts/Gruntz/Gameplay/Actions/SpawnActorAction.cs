using Base.Actors;
using Gruntz.Actors;
using UnityEngine;

namespace Gruntz.Gameplay.Actions
{
    public class SpawnActorAction : IGameplayAction
    {
        public ActorTemplateDef Template;
        public Vector3 Pos;
        public Actor Actor { get; set; }
    }
}
