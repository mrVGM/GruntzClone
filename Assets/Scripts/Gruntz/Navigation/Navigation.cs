using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace Gruntz.Navigation
{
    public class Navigation : IOrderedUpdate
    {
        private struct OccupiedPosition : IOccupiedPosition
        {
            public bool IsValid => true;

            public Vector3 Pos { get; set; }
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

        private HashSet<Vector3> occupiedPositions { get; } = new HashSet<Vector3>();

        private void CalculateMoves()
        {
            float dt = Time.deltaTime;
            occupiedPositions.Clear();
            moveRequests.Sort((x, y) => {
                if (x.OccupiedPosition == null)
                {
                    return y.OccupiedPosition == null ? 0 : 1;
                }
                if (y.OccupiedPosition == null) 
                {
                    return x.OccupiedPosition == null ? 0 : -1;
                }

                Vector3 offsetX = x.OccupiedPosition.Pos - x.CurrentPos;
                Vector3 offsetY = y.OccupiedPosition.Pos - y.CurrentPos;

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
                var res = CalculateMove(request, dt * request.MoveSpeed, occupiedPositions);
                occupiedPositions.Add(res.OccupiedPosition.Pos);
                request.MoveResultCallback(res);
            }
            moveRequests.Clear();
        }

        private MoveRequestResult CalculateMove(MoveRequest request, float distance, IEnumerable<Vector3> occupied)
        {
            Vector3 currentTargetPos = request.OccupiedPosition != null ? request.OccupiedPosition.Pos : request.CurrentPos;

            if (distance < Eps)
            {
                return new MoveRequestResult
                {
                    PositionToMove = request.CurrentPos,
                    Direction = Vector3.zero,
                    OccupiedPosition = new OccupiedPosition { Pos = request.CurrentPos }
                };
            }

            var offset = request.TargetPos - request.CurrentPos;
            if (offset.sqrMagnitude <= Eps)
            {
                return new MoveRequestResult
                {
                    PositionToMove = request.TargetPos, 
                    Direction = Vector3.zero, 
                    OccupiedPosition = new OccupiedPosition { Pos = request.TargetPos }
                };
            }

            if ((currentTargetPos - request.CurrentPos).sqrMagnitude <= Eps) 
            {
                var step = map.GetPossibleMoves(currentTargetPos)
                    .OrderBy(x => occupied.Contains(x) ? float.PositiveInfinity : map.MoveCost(x, request.TargetPos)).FirstOrDefault();
                if ((step - currentTargetPos).sqrMagnitude <= Eps) 
                {
                    return new MoveRequestResult
                    {
                        PositionToMove = currentTargetPos,
                        Direction = Vector3.zero,
                        OccupiedPosition = new OccupiedPosition { Pos = step }
                    };
                }
                if ((step - currentTargetPos).sqrMagnitude < distance * distance)
                {
                    float distanceLeft = distance - (step - currentTargetPos).magnitude;
                    return CalculateMove(new MoveRequest {
                        CurrentPos = step,
                        TargetPos = request.TargetPos,
                        MoveSpeed = request.MoveSpeed,
                        OccupiedPosition = new OccupiedPosition { Pos = step },
                        MoveResultCallback = request.MoveResultCallback
                    }, distanceLeft, occupied);
                }

                return CalculateMove(new MoveRequest
                {
                    CurrentPos = request.CurrentPos,
                    TargetPos = request.TargetPos,
                    MoveSpeed = request.MoveSpeed,
                    CurrentDirection = (step - currentTargetPos).normalized,
                    OccupiedPosition = new OccupiedPosition { Pos = step },
                    MoveResultCallback = request.MoveResultCallback
                }, distance, occupied);
            }

            if ((request.CurrentPos - currentTargetPos).sqrMagnitude < distance * distance) 
            {
                var distanceLeft = distance - (request.CurrentPos - currentTargetPos).magnitude;
                return CalculateMove(new MoveRequest
                {
                    CurrentPos = currentTargetPos,
                    TargetPos = request.TargetPos,
                    MoveSpeed = request.MoveSpeed,
                    OccupiedPosition = new OccupiedPosition { Pos = currentTargetPos },
                    MoveResultCallback = request.MoveResultCallback
                }, distanceLeft, occupied);
            }

            Vector3 direction = currentTargetPos - request.CurrentPos;
            direction.Normalize();
            Vector3 pos = request.CurrentPos + distance * direction;
            var closestPositions = GetEnclosingPositions(pos);
            Vector3 closest = closestPositions.OrderBy(x => (pos - x).sqrMagnitude).FirstOrDefault();
            if ((pos - closest).sqrMagnitude < Eps) 
            {
                pos = closest;
            }
            return new MoveRequestResult
            {
                PositionToMove = pos,
                Direction = direction,
                OccupiedPosition = new OccupiedPosition { Pos = currentTargetPos }
            };
        }

        public void DoUpdate()
        {
            CalculateMoves();
        }

        public Navigation()
        {
            var mainUpdater = Game.Instance.GetComponent<MainUpdater>();
            mainUpdater.RegisterUpdatable(this);
        }

        IEnumerable<Vector3> GetEnclosingPositions(Vector3 pos)
        {
            float floorX = Mathf.Floor(pos.x) + 0.5f;
            while (floorX <= pos.x) { floorX += 1; }
            while (floorX > pos.x) { floorX -= 1; }

            float floorZ = Mathf.Floor(pos.z) + 0.5f;
            while (floorZ <= pos.z) { floorZ += 1; }
            while (floorZ > pos.z) { floorZ -= 1; }


            Vector3 floor = floorX * Vector3.right + floorZ * Vector3.forward;
            yield return floor;
            yield return floor + Vector3.right;
            yield return floor + Vector3.right + Vector3.forward;
            yield return floor + Vector3.forward;
        }

        private Vector3 GetCurrentTargetPos(MoveRequest request)
        {
            if (request.CurrentDirection.sqrMagnitude < Eps)
            {
                return request.CurrentPos;
            }

            var enc = GetEnclosingPositions(request.CurrentPos);
            var positions = GetEnclosingPositions(request.CurrentPos).Where(x => Vector3.Dot(x - request.CurrentPos, request.CurrentDirection) > 0);
            positions = positions.Where(x => Vector3.Cross(x - request.CurrentPos, request.CurrentDirection).sqrMagnitude < Eps);
            return positions.FirstOrDefault();
        }
    }
}
