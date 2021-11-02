using Gruntz.Actors;
using UnityEngine;

namespace Gruntz.Gameplay
{
    public class SpawnActorGameplayEvent : GameplayEvent
    {
        public ActorTemplateDef Template;
        public Vector3 Pos;
    }
}
