using System;
using System.Collections.Generic;

namespace Base.Status
{
    [Serializable]
    public class StatusComponentData : ISerializedObjectData
    {
        public List<StatusData> StatusDatas = new List<StatusData>();
    }
}
