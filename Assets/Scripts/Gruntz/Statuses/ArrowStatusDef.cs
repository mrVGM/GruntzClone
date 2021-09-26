using Base.Status;

namespace Gruntz.Statuses
{
    public class ArrowStatusDef : StatusDef
    {
        protected override StatusData StatusData
        {
            get
            {
                var arrowStatusData = new ArrowStatusData();
                return arrowStatusData;
            }
        }
    }
}
