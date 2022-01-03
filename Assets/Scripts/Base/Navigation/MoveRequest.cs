using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Navigation
{
    public interface INavMoves
    {
        IEnumerable<Vector3> GetPossibleMoves(Vector3 pos);
    }
    public struct MoveRequest
    {
        public LayerMask Obstacles;
        public Vector3 CurrentPos;
        public INavigationTarget TargetPos;
        public float MoveSpeed;
        public bool CheckForSegmentInfoClashes;
        public ITravelSegmentInfo TravelSegmentInfo;
        public Action<MoveRequestResult> MoveResultCallback;
        public INavMoves NavMoves;
    }

    public struct MoveRequestResult
    {
        public Vector3 PositionToMove;
        public Vector3 Direction;
        public ITravelSegmentInfo TravelSegmentInfo;
        public Vector3[] TouchedPositions;
    }
}
