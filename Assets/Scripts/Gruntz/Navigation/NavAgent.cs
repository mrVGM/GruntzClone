using System.Linq;
using Base;
using UnityEngine;

namespace Gruntz.Navigation
{
    public class NavAgent : MonoBehaviour, IOrderedUpdate
    {
        private class TravelSegmentInfo : ITravelSegmentInfo
        {
            public NavAgent _navAgent;
            public TravelSegmentInfo(NavAgent navAgent)
            {
                _navAgent = navAgent;
            }

            public Vector3 EndPos => _navAgent.NavObstacle.transform.position;

            public Vector3 StartPos => _navAgent.LocalTravelStartPoint.position;
        }

        public Vector3 Target;

        public Collider NavObstacle;
        public Transform LocalTravelStartPoint;

        public Transform ActorVisuals;

        public NavAgentDef NavAgentDef;
        private NavAgentDef.NavAgentStats navStats = null;

        private TravelSegmentInfo _travelSegmentInfo;
        private TravelSegmentInfo travelSegmentInfo
        {
            get
            {
                if (_travelSegmentInfo == null)
                {
                    _travelSegmentInfo = new TravelSegmentInfo(this);
                }
                return _travelSegmentInfo;
            }
        }

        public NavAgentDef.NavAgentStats NavStats => navStats ?? NavAgentDef.NavStats;

        public ExecutionOrderTagDef OrderTagDef 
        {
            get
            {
                var game = Game.Instance;
                var defRepo = game.DefRepositoryDef;
                return defRepo.AllDefs.OfType<NavigationRequestOrderTagDef>().FirstOrDefault();
            }
        }

        public void DoUpdate()
        {
            var game = Game.Instance;
            var navDef = game.DefRepositoryDef.AllDefs.OfType<NavigationDef>().FirstOrDefault();
            var context = game.Context;
            var navigation = context.GetRuntimeObject(navDef) as Navigation;

            var request = new MoveRequest
            {
                CurrentPos = ActorVisuals.position,
                TargetPos = Target,
                MoveSpeed = NavStats.Speed,
                TravelSegmentInfo = new TravelSegmentInfo(this),
                MoveResultCallback = Move
            };

            navigation.MakeMoveRequest(request);
        }

        public void Init()
        {
            NavObstacle.transform.position = ActorVisuals.position;
            LocalTravelStartPoint.position = ActorVisuals.position;
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }

        private void Move(MoveRequestResult moveRequestResult) 
        {
            ActorVisuals.position = moveRequestResult.PositionToMove;
            NavObstacle.transform.position = moveRequestResult.TravelSegmentInfo.EndPos;
            LocalTravelStartPoint.position = moveRequestResult.TravelSegmentInfo.StartPos;

            if (!Navigation.AreVectorsTheSame(moveRequestResult.Direction, Vector3.zero))
            {
                ActorVisuals.rotation = Quaternion.LookRotation(moveRequestResult.Direction);
            }
        }
    }
}
