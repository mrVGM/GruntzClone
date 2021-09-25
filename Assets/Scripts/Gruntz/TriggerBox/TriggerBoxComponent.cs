using Gruntz.Actors;
using Gruntz.Status;
using System.Linq;
using UnityEngine;

namespace Gruntz
{
    public class TriggerBoxComponent : IActorComponent
    {
        public TriggerBoxActionDef TriggerBoxActionDef { get; }
        public Actor Actor { get; }
        public TriggerBoxBehaviour TriggerBox { get; }
        public TriggerBoxComponent(TriggerBoxBehaviour triggerBoxBehaviour, TriggerBoxActionDef triggerBoxAction, Actor actor)
        {
            TriggerBox = triggerBoxBehaviour;
            TriggerBoxActionDef = triggerBoxAction;
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
            TriggerBoxActionDef.TriggerEnter(TriggerBox.GetComponent<Collider>(), collider);
        }
        public void TriggerExited(Collider collider)
        {
            TriggerBoxActionDef.TriggerExit(TriggerBox.GetComponent<Collider>(), collider);
        }
    }
}
