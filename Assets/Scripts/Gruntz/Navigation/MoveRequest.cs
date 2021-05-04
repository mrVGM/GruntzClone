using System;
using UnityEngine;

namespace Gruntz.Navigation
{
    public struct MoveRequest
    {
        public Vector3 CurrentPos;
        public Vector3 CurrentDirection;
        public Vector3 TargetPos;
        public float MoveSpeed;
        public IOccupiedPosition OccupiedPosition;
        public Action<MoveRequestResult> MoveResultCallback;
    }

    public struct MoveRequestResult
    {
        public Vector3 PositionToMove;
        public Vector3 Direction;
        public IOccupiedPosition OccupiedPosition;
    }
}
