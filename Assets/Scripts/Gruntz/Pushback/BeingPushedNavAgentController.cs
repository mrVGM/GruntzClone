using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Actors;
using Base.Gameplay;
using Base.MessagesSystem;
using Base.Navigation;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Pushback
{
    [Serializable]
    public class BeingPushedNavAgentController : INavAgentController
    {
        private struct NavMoves : INavMoves
        {
            public BeingPushedNavAgentController Controller { get; }
            private IEnumerable<Vector3> Steps()
            {
                yield return Controller._pushSnappedOrigin + Vector3.back;
                yield return Controller._pushSnappedOrigin + Vector3.right;
                yield return Controller._pushSnappedOrigin + Vector3.right + Vector3.forward;
                yield return Controller._pushSnappedOrigin + Vector3.forward;
                yield return Controller._pushSnappedOrigin + Vector3.forward + Vector3.left;
                yield return Controller._pushSnappedOrigin + Vector3.left;
                yield return Controller._pushSnappedOrigin + Vector3.left + Vector3.back;
                yield return Controller._pushSnappedOrigin + Vector3.back + Vector3.right;
            }
            public IEnumerable<Vector3> GetPossibleMoves(Vector3 pos)
            {
                yield return pos;

                var potentialMoves = Steps();

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

            public NavMoves(BeingPushedNavAgentController controller)
            {
                Controller = controller;
            }
        }

        [NonSerialized]
        private NavAgent _navAgent;
        
        private DefRef<BeingPushedNavAgentControllerDef> _def;
        private SerializedVector3 _pushSnappedOrigin;
        private SerializedVector3 _pushDestination;
        private bool _pushProcessed;

        public BeingPushedNavAgentControllerDef BeingPushedNavAgentControllerDef => _def;

        public NavAgent NavAgent
        {
            set
            {
                _navAgent = value;
            }
        }

        public BeingPushedNavAgentController(BeingPushedNavAgentControllerDef def, NavAgent navAgent, Vector3 pushDestination, Vector3 pushSnappedOrigin)
        {
            _navAgent = navAgent;
            _pushDestination = pushDestination;
            _pushSnappedOrigin = pushSnappedOrigin;
            _pushProcessed = false;
            _def = def.ToDefRef<BeingPushedNavAgentControllerDef>();
        }
        public MoveRequest MoveRequest
        {
            get
            {
                ITravelSegmentInfo segmentInfo = _navAgent.TravelSegment;
                if (!_pushProcessed) {
                    segmentInfo = new TravelSegmentInfo { StartPos = _navAgent.Pos, EndPos = _navAgent.Pos };
                }
                var _navAgentData = _navAgent.Data as NavAgentData;
                var request = new MoveRequest
                {
                    Obstacles = _navAgentData.OriginalObstacles,
                    CurrentPos = _navAgent.Pos,
                    TargetPos = new SimpleNavTarget { Target = _pushDestination },
                    MoveSpeed = _navAgentData.Speed,
                    TravelSegmentInfo = segmentInfo,
                    CheckForSegmentInfoClashes = _navAgentData.CheckForSegmentInfoClashes,
                    MoveResultCallback = Move,
                    NavMoves = new NavMoves(this),
                };

                return request;
            }
        }

        private void Move(MoveRequestResult moveRequestResult)
        {
            if (!_pushProcessed && !Base.Navigation.Navigation.AreVectorsTheSame(_pushDestination, moveRequestResult.TravelSegmentInfo.EndPos)) {
                _navAgent.StopThePush();
                return;
            }
            if (!_pushProcessed) {
                _navAgent.Target = new SimpleNavTarget { Target = _pushDestination };
            }
            _pushProcessed = true;
            if (Base.Navigation.Navigation.AreVectorsTheSame(moveRequestResult.PositionToMove, _pushDestination)) {
                _navAgent.StopThePush();
            }
            
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            messagesSystem.SendMessage(_navAgent.NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, _navAgent.Actor, NavAgent.NavAgentState.Statying);
            
            _navAgent.SetTravelSegmentAndLocation(moveRequestResult.PositionToMove, Vector3.zero, moveRequestResult.TravelSegmentInfo);

            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            gameplayManager.HandleGameplayEvent(new ActorTouchedPositionGameplayEvent
            {
                Actor = _navAgent.Actor,
                Positions = moveRequestResult.TouchedPositions
            });
        }
    }
}
