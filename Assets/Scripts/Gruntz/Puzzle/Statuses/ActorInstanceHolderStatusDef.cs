using Base.Status;

namespace Gruntz.Puzzle.Statuses
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
