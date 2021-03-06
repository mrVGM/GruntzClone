using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Navigation
{
    [Serializable]
    public class NavAgentData : ISerializedObjectData
    {
        private List<string> _disabledObstacleLayers;
        public List<string> DisabledObstacleLayers
        {
            get
            {
                if (_disabledObstacleLayers == null) {
                    _disabledObstacleLayers = new List<string>();
                }
                return _disabledObstacleLayers;
            }
        }
        public string[] ObstacleLayers;
        public IEnumerable<string> ObstacleNames => ObstacleLayers.Except(DisabledObstacleLayers);
        public LayerMask Obstacles => LayerMask.GetMask(ObstacleNames.ToArray());
        public LayerMask OriginalObstacles => LayerMask.GetMask(ObstacleLayers);
        public SerializedVector3 InitialPosition;
        public INavigationTarget Target;
        public SerializedVector3 TravelSegmentStart;
        public SerializedVector3 TravelSegmentEnd;
        public bool CheckForSegmentInfoClashes => ObstacleNames.Contains(Utils.UnityLayers.UnitObstacle);
        public float Speed;

        public INavAgentController NavAgentController;
    }
}
