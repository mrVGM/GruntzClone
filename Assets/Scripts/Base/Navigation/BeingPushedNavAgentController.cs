using Base.Gameplay;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Base.Navigation
{
    public class BeingPushedNavAgentController : INavAgentController
    {
        private NavAgent _navAgent;
        private Vector3 _pushSnappedOrigin;
        private Vector3 _pushDestination;
        private bool _pushProcessed;

        public BeingPushedNavAgentController(NavAgent navAgent, Vector3 pushDestination, Vector3 pushSnappedOrigin)
        {
            _navAgent = navAgent;
            _pushDestination = pushDestination;
            _pushSnappedOrigin = pushSnappedOrigin;
            _pushProcessed = false;
        }
        public MoveRequest MoveRequest
        {
            get
            {
                ITravelSegmentInfo segmentInfo = _navAgent.TravelSegment;
                if (!_pushProcessed) {
                    segmentInfo = new TravelSegmentInfo { StartPos = _pushSnappedOrigin, EndPos = _pushSnappedOrigin };
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
                    MoveResultCallback = Move
                };

                return request;
            }
        }

        private void Move(MoveRequestResult moveRequestResult)
        {
            if (!_pushProcessed && !Navigation.AreVectorsTheSame(_pushDestination, moveRequestResult.TravelSegmentInfo.EndPos)) {
                _navAgent.StopThePush();
                return;
            }
            if (!_pushProcessed) {
                _navAgent.Target = new SimpleNavTarget { Target = _pushDestination };
            }
            _pushProcessed = true;
            if (Navigation.AreVectorsTheSame(moveRequestResult.PositionToMove, _pushDestination)) {
                _navAgent.StopThePush();
            }
            
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
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
