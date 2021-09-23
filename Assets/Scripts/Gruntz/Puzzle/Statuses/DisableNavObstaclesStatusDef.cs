using Gruntz.Status;
using System;

namespace Gruntz.Puzzle.Statuses
{
    [Serializable]
    public class DisableNavObstaclesStatusDef : Status.StatusDef
    {
        public string[] DisabledObstacleLayers;

        protected override StatusData StatusData
        {
            get
            {
                return new DisableNavObstaclesStatusData();
            }
        }
    }
}
