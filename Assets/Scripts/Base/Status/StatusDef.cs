using System;

namespace Base.Status
{
    public class StatusDef : Def
    {
        public StatusData Data
        {
            get
            {
                var statusData = StatusData;
                statusData.StatusDef = ToDefRef<StatusDef>();
                if (statusData.StatusId == null) {
                    statusData.StatusId = Guid.NewGuid().ToString();
                }
                return statusData;
            }
        }

        protected virtual StatusData StatusData => new StatusData();
    }
}
