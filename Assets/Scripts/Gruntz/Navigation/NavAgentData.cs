using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;
using Utils;

namespace Gruntz.Navigation
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
        public LayerMask Obstacles => LayerMask.GetMask(ObstacleLayers.Except(DisabledObstacleLayers).ToArray());
        public SerializedVector3 InitialPosition;
        public SerializedVector3 Target;
        public SerializedVector3 TravelSegmentStart;
        public SerializedVector3 TravelSegmentEnd;
        public bool CheckForSegmentInfoClashes => DisabledObstacleLayers.Contains(UnityLayers.UnitObstacle);
        public float Speed;
    }
}
