using Base.Status;
using System;

namespace Gruntz.Puzzle.Statuses
{
    [Serializable]
    public class DisableNavObstaclesStatusData : StatusData
    {
        public string AssociatedStatusId;
    }
}
