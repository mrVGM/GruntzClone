using Base;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public abstract class TriggerBoxActionDef : Def
    {
        public abstract void TriggerEnter(Collider ownCollider, Collider otherCollider);
        public abstract void TriggerExit(Collider ownCollider, Collider otherCollider);
    }
}
