using Base.Actors;
using Base.MessagesSystem;

namespace Gruntz
{
    public class TriggerArrowStatusActionDef : TriggerStatusActionDef
    {
        ArrowStatusDef ArrowStatusDef => StatusDef as ArrowStatusDef;
        public override Status.Status GetStatus(Actor source, Actor target)
        {
            var arrowStatusData = ArrowStatusDef.Data as ArrowStatusData;
            var destinationBehaviour = source.ActorComponent.GetComponent<ArrowDestinationBehaviour>();
            arrowStatusData.Destination = destinationBehaviour.Destination.position;
            arrowStatusData.Anchor = source.Pos;
            return arrowStatusData.CreateStatus();
        }
    }
}
