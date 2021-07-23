using Base;
using System;
using System.Collections.Generic;

namespace Gruntz.Status
{
    [Serializable]
    public class StatusComponentData : ISerializedObjectData
    {
        public List<StatusData> StatusDatas = new List<StatusData>();
    }
}
