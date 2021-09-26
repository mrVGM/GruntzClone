using Base.Status;

namespace Gruntz.Statuses
{
    public class ActorInstanceHolderStatusDef : StatusDef
    {
        protected override StatusData StatusData
        {
            get
            {
                return new ActorInstanceHolderStatusData();
            }
        }
    }
}
