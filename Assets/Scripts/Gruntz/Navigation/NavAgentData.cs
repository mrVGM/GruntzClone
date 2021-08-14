using System;
using Base;
using UnityEngine;
using Utils;

namespace Gruntz.Navigation
{
    [Serializable]
    public class NavAgentData : ISerializedObjectData
    {
        public string[] ObstacleLayers;
        public LayerMask Obstacles => LayerMask.GetMask(ObstacleLayers);
        public SerializedVector3 InitialPosition;
        public SerializedVector3 Target;
        public SerializedVector3 TravelSegmentStart;
        public SerializedVector3 TravelSegmentEnd;
        public float Speed;
    }
}
