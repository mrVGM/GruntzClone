using System;
using Base.Status;
using Utils;

namespace Gruntz.Statuses
{
    [Serializable]
    public class ArrowStatusData : StatusData
    {
        public SerializedVector3 Destination;
        public SerializedVector3 Anchor;
    }
}
