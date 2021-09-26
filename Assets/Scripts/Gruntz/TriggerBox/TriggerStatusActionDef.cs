using Base.Actors;
using Base.Status;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class TriggerStatusActionDef : TriggerBoxActionDef
    {
        public StatusDef StatusDef;
        public bool RemoveStatus = true;
        public virtual Status GetStatus(Actor ownActor, Actor otherActor)
        {
            return StatusDef.Data.CreateStatus();
        }

        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var actorProxy = otherCollider.GetComponent<ActorProxy>();
            if (actorProxy == null) {
                return;
            }
            var actor = actorProxy.Actor;
            var statusComponent = actor.GetComponent<StatusComponent>();
            var status = GetStatus(ownActor, actor);
            statusComponent.AddStatus(status);
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
            if (!RemoveStatus) {
                return;
            }
            
            var actorProxy = otherCollider.GetComponent<ActorProxy>();
            if (actorProxy == null) {
                return;
            }
            var actor = actorProxy.Actor;
            var statusComponent = actor.GetComponent<StatusComponent>();
            var status = statusComponent.GetStatus(StatusDef);
            statusComponent.RemoveStatus(status);
        }
    }
}
