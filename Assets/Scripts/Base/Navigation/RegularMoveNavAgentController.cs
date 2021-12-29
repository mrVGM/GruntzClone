using Base.Gameplay;
using static Base.Navigation.NavAgent;

namespace Base.Navigation
{
    public class RegularMoveNavAgentController : INavAgentController
    {
        private NavAgent _navAgent;

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
                    MoveResultCallback = Move
                };

                return request;
            }
        }

        private void Move(MoveRequestResult moveRequestResult)
        {
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
            if ((_navAgent.Pos - moveRequestResult.PositionToMove).sqrMagnitude > 0.001) {
                messagesSystem.SendMessage(_navAgent.NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, _navAgent.Actor, NavAgent.NavAgentState.Moving);
            }
            else {
                messagesSystem.SendMessage(_navAgent.NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, _navAgent.Actor, NavAgent.NavAgentState.Statying);
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
