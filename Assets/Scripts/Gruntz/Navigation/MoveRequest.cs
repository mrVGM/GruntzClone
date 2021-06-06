using System;
using UnityEngine;

namespace Gruntz.Navigation
{
    public struct MoveRequest
    {
        public Vector3 CurrentPos;
        public Vector3 TargetPos;
        public float MoveSpeed;
        public ITravelSegmentInfo TravelSegmentInfo;
        public Action<MoveRequestResult> MoveResultCallback;
    }

    public struct MoveRequestResult
    {
        public Vector3 PositionToMove;
        public Vector3 Direction;
        public ITravelSegmentInfo TravelSegmentInfo;
    }
}