using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Navigation
{
    public class Navigation : IOrderedUpdate, IContextObject
    {
        private const float Eps = 0.000001f;
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
        public static bool AreVectorsTheSame(Vector3 x, Vector3 y)
        {
            return (x - y).sqrMagnitude < Eps;
        }

        private void CalculateMoves(IEnumerable<MoveRequest> moveRequests, float dt)
        {
            foreach (var moveRequest in moveRequests)
            {
                var travelSegments = moveRequests.Where(x => x.CheckForSegmentInfoClashes).Select(x => x.TravelSegmentInfo).Where(x => x != moveRequest.TravelSegmentInfo);
                float maxTravelDistance = dt * moveRequest.MoveSpeed;

                var moveRequestResult = CalculateMove(moveRequest, travelSegments, maxTravelDistance);
                moveRequest.MoveResultCallback(moveRequestResult);
            }
        }

        private MoveRequestResult CalculateMove(MoveRequest moveRequest, IEnumerable<ITravelSegmentInfo> travelSegments, float maxTravelDistance)
        {
            if (AreVectorsTheSame(moveRequest.CurrentPos, moveRequest.TargetPos))
            {
                var moveRequestResult = new MoveRequestResult
                {
                    PositionToMove = moveRequest.TargetPos,
                    Direction = Vector3.zero,
                    TravelSegmentInfo = new TravelSegmentInfo { StartPos = moveRequest.TargetPos, EndPos = moveRequest.TargetPos },
                    TouchedPositions = new[] { moveRequest.TargetPos }
                };

                return moveRequestResult;
            }

            if (AreVectorsTheSame(moveRequest.CurrentPos, moveRequest.TravelSegmentInfo.EndPos))
            {
                Vector3 currentPos = moveRequest.TravelSegmentInfo.EndPos;
                var possibleSteps = map.GetPossibleMoves(currentPos, moveRequest.Obstacles).ToArray();
                var bestStep = possibleSteps.OrderBy(x => {
                    if (moveRequest.CheckForSegmentInfoClashes)
                    {
                        var currentTravelSegment = new TravelSegmentInfo
                            {StartPos = moveRequest.TravelSegmentInfo.StartPos, EndPos = x};
                        if (travelSegments.Any(y => !TravelSegmentInfoUtils.AreCompatible(currentTravelSegment, y)))
                        {
                            return float.PositiveInfinity;
                        }
                    }

                    return map.MoveCost(x, moveRequest.TargetPos);
                }).First();

                if (AreVectorsTheSame(bestStep, currentPos))
                {
                    var moveRequestResult = new MoveRequestResult
                    {
                        PositionToMove = currentPos,
                        Direction = Vector3.zero,
                        TravelSegmentInfo = new TravelSegmentInfo { StartPos = currentPos, EndPos = currentPos },
                        TouchedPositions = new[] { currentPos }
                    };
                    return moveRequestResult;
                }

                Vector3 dir = (bestStep - currentPos).normalized;
                if ((bestStep - currentPos).sqrMagnitude <= maxTravelDistance * maxTravelDistance)
                {
                    var moveReq = new MoveRequest
                    {
                        Obstacles = moveRequest.Obstacles,
                        CurrentPos = bestStep,
                        MoveSpeed = moveRequest.MoveSpeed,
                        TargetPos = moveRequest.TargetPos,
                        TravelSegmentInfo = new TravelSegmentInfo { StartPos = bestStep, EndPos = bestStep },
                        CheckForSegmentInfoClashes = moveRequest.CheckForSegmentInfoClashes,
                        MoveResultCallback = moveRequest.MoveResultCallback,
                    };
                    float dist = maxTravelDistance - (bestStep - currentPos).magnitude;
                    var res = CalculateMove(moveReq, travelSegments, dist);
                    if (res.TouchedPositions == null) {
                        res.TouchedPositions = new Vector3[0];
                    }
                    res.TouchedPositions = res.TouchedPositions.Append(bestStep).ToArray();
                    return res;
                }

                {
                    var moveRequestResult = new MoveRequestResult
                    {
                        PositionToMove = currentPos + maxTravelDistance * dir,
                        Direction = dir,
                        TravelSegmentInfo = new TravelSegmentInfo { StartPos = currentPos, EndPos = bestStep },
                        TouchedPositions = new[] { moveRequest.TravelSegmentInfo.EndPos },
                    };
                    return moveRequestResult;
                }
            }

            {
                Vector3 immediateTarget = moveRequest.TravelSegmentInfo.EndPos;
                Vector3 dir = (immediateTarget - moveRequest.CurrentPos).normalized;
                if ((moveRequest.CurrentPos - immediateTarget).sqrMagnitude <= maxTravelDistance * maxTravelDistance)
                {
                    var moveReq = new MoveRequest
                    {
                        Obstacles = moveRequest.Obstacles,
                        CurrentPos = immediateTarget,
                        MoveSpeed = moveRequest.MoveSpeed,
                        TargetPos = moveRequest.TargetPos,
                        TravelSegmentInfo = new TravelSegmentInfo { StartPos = immediateTarget, EndPos = immediateTarget },
                        CheckForSegmentInfoClashes = moveRequest.CheckForSegmentInfoClashes,
                        MoveResultCallback = moveRequest.MoveResultCallback,
                    };
                    float dist = maxTravelDistance - (moveRequest.CurrentPos - immediateTarget).magnitude;
                    var res = CalculateMove(moveReq, travelSegments, dist);

                    return res;
                }

                {
                    var moveRequestResult = new MoveRequestResult
                    {
                        PositionToMove = moveRequest.CurrentPos + maxTravelDistance * dir,
                        Direction = dir,
                        TravelSegmentInfo = new TravelSegmentInfo { StartPos = moveRequest.TravelSegmentInfo.StartPos, EndPos = moveRequest.TravelSegmentInfo.EndPos }
                    };
                    return moveRequestResult;
                }
            }
        }
        private void CalculateMoves()
        {
            float dt = Time.fixedDeltaTime;
            CalculateMoves(moveRequests, dt);
            moveRequests.Clear();
        }
        public void DoUpdate(MainUpdaterUpdateTime updateTime)
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