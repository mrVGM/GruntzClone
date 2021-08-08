using Base.MessagesSystem;
using Gruntz.Actors;
using Gruntz.UserInteraction.UnitController;

namespace Gruntz
{
    public class TriggerArrowStatusActionDef : TriggerStatusActionDef
    {
        ArrowStatusDef ArrowStatusDef => StatusDef as ArrowStatusDef;
        public override Status.Status GetStatus(Actor source, Actor target)
        {
            var arrowStatusData = ArrowStatusDef.Data as ArrowStatusData;
            var destinationBehaviour = source.ActorComponent.GetComponent<ArrowDestinationBehaviour>();
            var previousUnitControllerChannel = target.GetComponent<UnitController>().MessagesBox;
            arrowStatusData.Destination = destinationBehaviour.Destination.position;
            arrowStatusData.PreviousUnitControllerChannel = previousUnitControllerChannel.ToDefRef<MessagesBoxTagDef>();
            return arrowStatusData.CreateStatus();
        }
    }
}
