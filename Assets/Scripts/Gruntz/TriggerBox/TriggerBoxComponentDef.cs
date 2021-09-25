using Gruntz.Actors;
using System.Linq;

namespace Gruntz
{
    public class TriggerBoxComponentDef : ActorComponentDef
    {
        public TriggerBoxActionDef TriggerBoxActionDef;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var triggerBoxBehaviour = actor.ActorComponent
                .GetComponentsInChildren<TriggerBoxBehaviour>().First(x => x.TriggerBoxComponentDef == this);
            return new TriggerBoxComponent(triggerBoxBehaviour, TriggerBoxActionDef, actor);
        }
    }
}
