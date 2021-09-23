using Base;
using System;

namespace Gruntz.Status
{
    [Serializable]
    public class StatusData : ISerializedObjectData
    {
        public DefRef<StatusDef> StatusDef;
        public string StatusId = null;

        public Status CreateStatus()
        {
            var status = new Status();
            status.Data = this;
            return status;
        }
    }
}
