using Base.Actors;
using Gruntz.Actors;
using Base.Status;
using System.Linq;
using UnityEngine;
using Gruntz.Statuses;

namespace Gruntz.Gameplay.Actions
{
    public class KillActorAction : IGameplayAction
    {
        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public Actor Actor { get; set; }
    }
}
