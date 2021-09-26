using System;
using UnityEngine;

namespace Base.Navigation
{
    public struct MoveRequest
    {
        public LayerMask Obstacles;
        public Vector3 CurrentPos;
        public Vector3 TargetPos;
        public float MoveSpeed;
        public bool CheckForSegmentInfoClashes;
        public ITravelSegmentInfo TravelSegmentInfo;
        public Action<MoveRequestResult> MoveResultCallback;
    }

    public struct MoveRequestResult
    {
        public Vector3 PositionToMove;
        public Vector3 Direction;
        public ITravelSegmentInfo TravelSegmentInfo;
        public Vector3[] TouchedPositions;
    }
}
