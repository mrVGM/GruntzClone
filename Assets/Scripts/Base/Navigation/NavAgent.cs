using System.Linq;
using Base.Actors;
using Gruntz.Gameplay;
using UnityEngine;

namespace Base.Navigation
{
    public class NavAgent : IActorComponent, IOrderedUpdate, ISerializedObject
    {
        public class ActorTouchedPositionGameplayEvent : GameplayEvent
        {
            public Actor Actor;
            public Vector3[] Positions;
        }
        private class TravelSegmentInfo : ITravelSegmentInfo
        {
            public NavAgent _navAgent;
            public TravelSegmentInfo(NavAgent navAgent)
            {
                _navAgent = navAgent;
            }

            public Vector3 EndPos => _navAgent._navAgentBehaviour.NavObstacle.transform.position;

            public Vector3 StartPos => _navAgent._navAgentBehaviour.LocalTravelStartPoint.position;
        }

        public Actor Actor { get; }
        public NavAgentComponentDef NavAgentComponentDef { get; }

        private NavAgentData _navAgentData;
        private NavAgentBehaviour _navAgentBehaviour;

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

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var game = Game.Instance;
                var defRepo = game.DefRepositoryDef;
                return defRepo.AllDefs.OfType<NavigationRequestOrderTagDef>().FirstOrDefault();
            }
        }

        public Vector3 Pos =>_navAgentBehaviour.ActorVisuals.position;

        public Vector3 Target
        {
            set
            {
                _navAgentData.Target = value;
            }
        }

        public ISerializedObjectData Data
        {
            get
            {
                _navAgentData.InitialPosition = Pos;
                _navAgentData.TravelSegmentStart = _navAgentBehaviour.LocalTravelStartPoint.position;
                _navAgentData.TravelSegmentEnd = _navAgentBehaviour.NavObstacle.transform.position;
                return _navAgentData;
            }
            set
            {
                _navAgentData = value as NavAgentData;
                SetObstacle();
            }
        }

        public NavAgent(NavAgentComponentDef navAgentComponentDef, Actor actor, NavAgentData navAgentData, NavAgentBehaviour navAgentBehaviour)
        {
            NavAgentComponentDef = navAgentComponentDef;
            _navAgentBehaviour = navAgentBehaviour;
            _navAgentData = navAgentData;
            Actor = actor;
        }
        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            var game = Game.Instance;
            var navDef = game.DefRepositoryDef.AllDefs.OfType<NavigationDef>().FirstOrDefault();
            var context = game.Context;
            var navigation = context.GetRuntimeObject(navDef) as Navigation;

            var request = new MoveRequest
            {
                Obstacles = _navAgentData.Obstacles,
                CurrentPos = _navAgentBehaviour.ActorVisuals.position,
                TargetPos = _navAgentData.Target,
                MoveSpeed = _navAgentData.Speed,
                TravelSegmentInfo = new TravelSegmentInfo(this),
                CheckForSegmentInfoClashes = _navAgentData.CheckForSegmentInfoClashes,
                MoveResultCallback = Move
            };

            navigation.MakeMoveRequest(request);
        }

        public void Init()
        {
            SetObstacle();
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }

        private void SetObstacle()
        {
            _navAgentBehaviour.ActorVisuals.position = _navAgentData.InitialPosition;
            _navAgentBehaviour.NavObstacle.transform.position = _navAgentData.TravelSegmentEnd;
            _navAgentBehaviour.LocalTravelStartPoint.position = _navAgentData.TravelSegmentStart;
        }

        private void Move(MoveRequestResult moveRequestResult) 
        {
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
            if ((_navAgentBehaviour.ActorVisuals.position - moveRequestResult.PositionToMove).sqrMagnitude > 0.001) {
                messagesSystem.SendMessage(NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, Actor, "moving");
            }
            else {
                messagesSystem.SendMessage(NavAgentComponentDef.NavigationMessages, MainUpdaterUpdateTime.FixedCrt, Actor, "staying");
            }

            _navAgentBehaviour.ActorVisuals.position = moveRequestResult.PositionToMove;
            _navAgentBehaviour.NavObstacle.transform.position = moveRequestResult.TravelSegmentInfo.EndPos;
            _navAgentBehaviour.LocalTravelStartPoint.position = moveRequestResult.TravelSegmentInfo.StartPos;

            if (!Navigation.AreVectorsTheSame(moveRequestResult.Direction, Vector3.zero))
            {
                _navAgentBehaviour.ActorVisuals.rotation = Quaternion.LookRotation(moveRequestResult.Direction);
            }

            var gameplayManager = GameplayManager.GetActorManagerFromContext();
            gameplayManager.HandleGameplayEvent(new ActorTouchedPositionGameplayEvent {
                Actor = Actor,
                Positions = moveRequestResult.TouchedPositions
            });
        }

        public void DeInit()
        {
            var game = Game.Instance;
            game.MainUpdater.UnRegisterUpdatable(this);
        }
    }
}