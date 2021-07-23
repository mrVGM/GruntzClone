using Base;

namespace Gruntz.Status
{
    public class StatusDef : Def
    {
        public StatusData Data
        {
            get
            {
                var statusData = StatusData;
                statusData.StatusDef = ToDefRef<StatusDef>();
                return statusData;
            }
        }

        protected virtual StatusData StatusData => new StatusData();
    }
}
