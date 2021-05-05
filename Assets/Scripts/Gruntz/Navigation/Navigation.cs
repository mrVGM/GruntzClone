using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace Gruntz.Navigation
{
    public class Navigation : IOrderedUpdate, IContextObject
    {
        private struct TravelSegmentInfo : ITravelSegmentInfo
        {
            public Vector3 Pos { get; set; }
            public Vector3 StartPos { get; set; }
        }

        public const float Eps = 0.000001f;
        private List<MoveRequest> moveRequests { get; } = new List<MoveRequest>();

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var game = Game.Instance;
                var defRepo = game.DefRepositoryDef;
                return defRepo.AllDefs.OfType<NavigationExecutionOrderTagDef>().FirstOrDefault();
            }
        }

        public void MakeMoveRequest(MoveRequest moveRequest)
        {
            moveRequests.Add(moveRequest);
        }

        private Map map = new Map();

        private HashSet<ITravelSegmentInfo> travelSegments { get; } = new HashSet<ITravelSegmentInfo>();

        private void CalculateMoves()
        {
            float dt = Time.deltaTime;
            travelSegments.Clear();
            moveRequests.Sort((x, y) => {
                if (x.TravelSegmentInfo == null)
                {
                    return y.TravelSegmentInfo == null ? 0 : 1;
                }
                if (y.TravelSegmentInfo == null) 
                {
                    return x.TravelSegmentInfo == null ? 0 : -1;
                }

                Vector3 offsetX = x.TravelSegmentInfo.Pos - x.CurrentPos;
                Vector3 offsetY = y.TravelSegmentInfo.Pos - y.CurrentPos;

                bool isXInPlace = offsetX.sqrMagnitude < Eps;
                bool isYInPlace = offsetY.sqrMagnitude < Eps;

                if (isXInPlace) 
                {
                    return isYInPlace ? 0 : 1;
                }

                if (isYInPlace) 
                {
                    return isXInPlace ? 0 : -1;
                }
                return 0;
            });
            foreach (var request in moveRequests)
            {
                var res = CalculateMove(request, dt * request.MoveSpeed, travelSegments);
                travelSegments.Add(res.TravelSegmentInfo);
                request.MoveResultCallback(res);
            }
            moveRequests.Clear();
        }

        private MoveRequestResult CalculateMove(MoveRequest request, float distance, IEnumerable<ITravelSegmentInfo> travels)
        {
            Vector3 currentTargetPos = request.TravelSegmentInfo != null ? request.TravelSegmentInfo.Pos : request.CurrentPos;
            Vector3 currentTravelStart = request.TravelSegmentInfo != null ? request.TravelSegmentInfo.StartPos : request.CurrentPos;

            var offset = request.TargetPos - request.CurrentPos;
            if (offset.sqrMagnitude <= Eps)
            {
                return new MoveRequestResult
                {
                    PositionToMove = request.TargetPos, 
                    Direction = Vector3.zero, 
                    TravelSegmentInfo = new TravelSegmentInfo { Pos = request.TargetPos, StartPos = request.TargetPos }
                };
            }

            if ((currentTargetPos - request.CurrentPos).sqrMagnitude <= Eps) 
            {
                bool areCrossing(Vector3 v1, Vector3 v2, Vector3 w1, Vector3 w2)
                {
                    Vector3 cross1 = Vector3.Cross(v2 - v1, w1 - v1);
                    Vector3 cross2 = Vector3.Cross(v2 - v1, w2 - v1);

                    if (Vector3.Dot(cross1, cross2) >= 0) { return false; }

                    cross1 = Vector3.Cross(w2 - w1, v1 - w1);
                    cross2 = Vector3.Cross(w2 - w1, v2 - w1);

                    if (Vector3.Dot(cross1, cross2) >= 0) { return false; }

                    return true;
                }
                var step = map.GetPossibleMoves(currentTargetPos)
                    .OrderBy(x => {
                        if (travels.Any(y => (y.Pos - x).sqrMagnitude < Eps)) { return float.PositiveInfinity; }
                        if (travels.Any(y => areCrossing(y.StartPos, y.Pos, x, currentTargetPos))) { return float.PositiveInfinity; }
                        return map.MoveCost(x, request.TargetPos); 
                    }).FirstOrDefault();
                if ((step - currentTargetPos).sqrMagnitude <= Eps) 
                {
                    return new MoveRequestResult
                    {
                        PositionToMove = currentTargetPos,
                        Direction = Vector3.zero,
                        TravelSegmentInfo = new TravelSegmentInfo { Pos = step, StartPos = step }
                    };
                }
                if ((step - currentTargetPos).sqrMagnitude < distance * distance)
                {
                    float distanceLeft = distance - (step - currentTargetPos).magnitude;
                    return CalculateMove(new MoveRequest {
                        CurrentPos = step,
                        TargetPos = request.TargetPos,
                        MoveSpeed = request.MoveSpeed,
                        TravelSegmentInfo = new TravelSegmentInfo { Pos = step, StartPos = step },
                        MoveResultCallback = request.MoveResultCallback
                    }, distanceLeft, travels);
                }

                return CalculateMove(new MoveRequest
                {
                    CurrentPos = request.CurrentPos,
                    TargetPos = request.TargetPos,
                    MoveSpeed = request.MoveSpeed,
                    CurrentDirection = (step - currentTargetPos).normalized,
                    TravelSegmentInfo = new TravelSegmentInfo { Pos = step, StartPos = currentTargetPos},
                    MoveResultCallback = request.MoveResultCallback
                }, distance, travels);
            }

            if ((request.CurrentPos - currentTargetPos).sqrMagnitude < distance * distance) 
            {
                var distanceLeft = distance - (request.CurrentPos - currentTargetPos).magnitude;
                return CalculateMove(new MoveRequest
                {
                    CurrentPos = currentTargetPos,
                    TargetPos = request.TargetPos,
                    MoveSpeed = request.MoveSpeed,
                    TravelSegmentInfo = new TravelSegmentInfo { Pos = currentTargetPos, StartPos = currentTargetPos },
                    MoveResultCallback = request.MoveResultCallback
                }, distanceLeft, travels);
            }

            Vector3 dirToCurrentTarget = currentTargetPos - request.CurrentPos;
            dirToCurrentTarget.Normalize();
            Vector3 pos = request.CurrentPos + distance * dirToCurrentTarget;
            if ((pos - currentTargetPos).sqrMagnitude < Eps)
            {
                pos = currentTargetPos;
            }
            return new MoveRequestResult
            {
                PositionToMove = pos,
                Direction = dirToCurrentTarget,
                TravelSegmentInfo = new TravelSegmentInfo { Pos = currentTargetPos, StartPos = currentTravelStart }
            };
        }

        public void DoUpdate()
        {
            CalculateMoves();
        }

        public void DisposeObject()
        {
        }

        public Navigation()
        {
            var mainUpdater = Game.Instance.MainUpdater;
            mainUpdater.RegisterUpdatable(this);
        }
    }
}
