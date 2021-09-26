using Base;
using Gruntz.Actors;
using Base.Status;
using UnityEngine;

namespace Gruntz
{
    public abstract class TriggerBoxActionDef : Def
    {
        public abstract void TriggerEnter(Collider ownCollider, Collider otherCollider);
        public abstract void TriggerExit(Collider ownCollider, Collider otherCollider);
    }
}
