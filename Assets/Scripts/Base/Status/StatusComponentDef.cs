using Base.Actors;

namespace Base.Status
{
    public class StatusComponentDef : ActorComponentDef
    {
        public StatusDef[] InitialStatuses;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var statusComponent = new StatusComponent(actor);
            foreach (var statusDef in InitialStatuses)
            {
                var status = statusDef.Data.CreateStatus();
                statusComponent.AddStatus(status);
            }
            return statusComponent;
        }
    }
}
