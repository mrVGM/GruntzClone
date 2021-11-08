using System;
using Base;
using Base.Status;

namespace Gruntz.Statuses
{
    [Serializable]
    public class ArrowStatusData : StatusData
    {
        public SerializedVector3 Destination;
        public SerializedVector3 Anchor;
    }
}
