using Gruntz.Actors;
using Gruntz.Status;
using UnityEngine;

namespace Gruntz
{
    public class TriggerBoxComponent : IActorComponent
    {
        public TriggerBoxComponentDef TriggerBoxComponentDef { get; }
        public Actor Actor { get; }
        public TriggerBoxBehaviour TriggerBox => Actor.ActorComponent.GetComponent<TriggerBoxBehaviour>();
        public TriggerBoxComponent(TriggerBoxComponentDef triggerBoxComponent, Actor actor)
        {
            TriggerBoxComponentDef = triggerBoxComponent;
            Actor = actor;
        }
        public void DeInit()
        {
            TriggerBox.TriggerEntered = null;
            TriggerBox.TriggerExited = null;
        }

        public void Init()
        {
            TriggerBox.TriggerEntered = TriggerEntered;
            TriggerBox.TriggerExited = TriggerExited;
        }

        public void TriggerEntered(Collider collider)
        {
            var actorProxy = collider.GetComponent<ActorProxy>();
            if (actorProxy == null)
            {
                return;
            }
            var actor = actorProxy.Actor;
            var statusComponent = actor.GetComponent<StatusComponent>();
            var status = statusComponent.GetStatus(TriggerBoxComponentDef.StatusDef);
            if (status != null)
            {
                return;
            }
            status = TriggerBoxComponentDef.StatusDef.Data.CreateStatus();
            statusComponent.AddStatus(status);
        }
        public void TriggerExited(Collider collider)
        {
            var actorProxy = collider.GetComponent<ActorProxy>();
            if (actorProxy == null)
            {
                return;
            }
            var actor = actorProxy.Actor;
            var statusComponent = actor.GetComponent<StatusComponent>();
            var status = statusComponent.GetStatus(TriggerBoxComponentDef.StatusDef);
            statusComponent.RemoveStatus(status);
        }
    }
}
