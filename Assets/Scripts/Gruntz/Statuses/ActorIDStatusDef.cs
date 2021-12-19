using Base.Status;

namespace Gruntz.Statuses
{
    public class ActorIDStatusDef : StatusDef
    {
        protected override StatusData StatusData
        {
            get
            {
                var actorIDStatusData = new ActorIDStatusData();
                return actorIDStatusData;
            }
        }
    }
}
