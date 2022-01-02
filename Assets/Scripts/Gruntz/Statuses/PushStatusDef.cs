using Base.Status;

namespace Gruntz.Statuses
{
    public class PushStatusDef : StatusDef
    {
        protected override StatusData StatusData
        {
            get
            {
                var pushStatusData = new PushStatusData();
                return pushStatusData;
            }
        }
    }
}
