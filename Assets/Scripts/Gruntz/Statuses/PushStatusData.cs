using System;
using Base;
using Base.Status;

namespace Gruntz.Statuses
{
    [Serializable]
    public class PushStatusData : StatusData
    {
        public SerializedVector3 PushDestination;
    }
}
