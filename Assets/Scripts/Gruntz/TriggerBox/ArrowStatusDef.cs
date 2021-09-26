using Base.Status;

namespace Gruntz
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
