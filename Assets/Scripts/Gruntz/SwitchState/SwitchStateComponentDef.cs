using Base.Actors;
using Base.Status;

namespace Gruntz.SwitchState
{
    public class SwitchStateComponentDef : ActorComponentDef
    {
        public StatusDef[] StateStatuses;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new SwitchStateComponent(this, actor);
        }
    }
}
