using Base.Status;
using System;

namespace Gruntz.Statuses
{
    [Serializable]
    public class DisableNavObstaclesStatusDef : StatusDef
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
