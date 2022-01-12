using System;
using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.Gameplay;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Base.Navigation
{
    [Serializable]
    public class RegularMoveNavAgentController : INavAgentController
    {
        private struct NavMoves : INavMoves
        {
            public RegularMoveNavAgentController Controller { get; }
            private IEnumerable<Vector3> Steps(Vector3 pos)
            {
                yield return pos + Vector3.back;
                yield return pos + Vector3.right;
                yield return pos + Vector3.right + Vector3.forward;
                yield return pos + Vector3.forward;
                yield return pos + Vector3.forward + Vector3.left;
                yield return pos + Vector3.left;
                yield return pos + Vector3.left + Vector3.back;
                yield return pos + Vector3.back + Vector3.right;
            }
            public IEnumerable<Vector3> GetPossibleMoves(Vector3 pos)
            {
                yield return pos;

                var potentialMoves = Steps(pos);

                foreach (var potentialMove in potentialMoves)
                {
                    var offset = potentialMove - pos;
                    var ray = new Ray(pos, offset);
                    IEnumerable<RaycastHit> hits = Physics.SphereCastAll(ray, .2f, offset.magnitude, (Controller._navAgent.Data as NavAgentData).Obstacles);

                    var actor = Controller._navAgent.Actor;
                    hits = hits.Where(x => {
                        var actorProxy = x.collider.GetComponent<ActorProxy>();
                        if (actorProxy == null) {
                            return true;
                        }
                        return actorProxy.Actor != actor;
                    });
                    if (hits.Any()) {
                        continue;
                    }
                    yield return potentialMove;
                }
            }

            public NavMoves(RegularMoveNavAgentController controller)
            {
                Controller = controller;
            }
        }

        [NonSerialized]
        private NavAgent _navAgent;

        public NavAgent NavAgent
        {
            set
            {
                _navAgent = value;
            }
        }
        public RegularMoveNavAgentController(NavAgent navAgent)
        {
            _navAgent = navAgent;
        }
        public MoveRequest MoveRequest
        {
            get
            {
                var navData = _navAgent.Data as NavAgentData;
                var request = new MoveRequest
                {
                    Obstacles = navData.Obstacles,
                    CurrentPos = _navAgent.Pos,
                    TargetPos = navData.Target,
                    MoveSpeed = navData.Speed,
                    TravelSegmentInfo = _navAgent.TravelSegment,
                    CheckForSegmentInfoClashes = navData.CheckForSegmentInfoClashes,
                    MoveResultCallback = Move,
                    NavMoves = new NavMoves(this),
                };

                return request;
            }
        }

        private void Move(MoveRequestResult moveRequestResult)
        {
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
            if ((_navAgent.Pos - moveRequestResult.PositionToMove).sqrMagnitude > 0.000001) {
                messagesSystem.SendMessage(_navAgent.NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, _navAgent.Actor, NavAgent.NavAgentState.Moving);
            }
            else {
                messagesSystem.SendMessage(_navAgent.NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, _navAgent.Actor, NavAgent.NavAgentState.Staying);
            }

            _navAgent.SetTravelSegmentAndLocation(moveRequestResult.PositionToMove, moveRequestResult.Direction, moveRequestResult.TravelSegmentInfo);

            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            gameplayManager.HandleGameplayEvent(new ActorTouchedPositionGameplayEvent {
                Actor = _navAgent.Actor,
                Positions = moveRequestResult.TouchedPositions
            });
        }
    }
}
