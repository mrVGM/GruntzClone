using Base.Status;
using System;

namespace Gruntz.Statuses
{
    [Serializable]
    public class DisableNavObstaclesStatusData : StatusData
    {
        public string AssociatedStatusId;
    }
}
