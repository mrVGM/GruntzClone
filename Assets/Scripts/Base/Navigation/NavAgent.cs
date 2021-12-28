using System;
using System.Linq;
using Base.Actors;
using Base.Gameplay;
using UnityEngine;

namespace Base.Navigation
{
    public class NavAgent : IActorComponent, IOrderedUpdate, ISerializedObject
    {
        private class Push
        {
            public Vector3 Destination;
        }

        [Serializable]
        public class SimpleNavTarget : INavigationTarget
        {
            public SerializedVector3 Target;

            public Vector3 AdjustPosition(Vector3 pos)
            {
                return Target;
            }

            public bool HasArrived(Vector3 pos)
            {
                return (pos - Target).sqrMagnitude < 0.0001;
            }

            public float Proximity(Vector3 pos)
            {
                return (pos - Target).magnitude;
            }
        }

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

        public INavigationTarget Target
        {
            set
            {
                _navAgentData.Target = value;
            }
        }

        public INavigationTarget NavTarget
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
            var navigation = Navigation.GetNavigationFromContext();

            var request = new MoveRequest {
                Obstacles = _navAgentData.Obstacles,
                CurrentPos = _navAgentBehaviour.ActorVisuals.position,
                TargetPos = _navAgentData.Target,
                MoveSpeed = _navAgentData.Speed,
                TravelSegmentInfo = new TravelSegmentInfo(this),
                CheckForSegmentInfoClashes = _navAgentData.CheckForSegmentInfoClashes,
                MoveResultCallback = Move
            };

            if (CurrentPush != null)
            {
                request = new MoveRequest {
                    Obstacles = _navAgentData.Obstacles,
                    CurrentPos = _navAgentBehaviour.ActorVisuals.position,
                    TargetPos = new SimpleNavTarget { Target = CurrentPush.Destination },
                    MoveSpeed = _navAgentData.Speed,
                    TravelSegmentInfo = new Base.Navigation.TravelSegmentInfo { StartPos = Pos, EndPos = Pos },
                    CheckForSegmentInfoClashes = _navAgentData.CheckForSegmentInfoClashes,
                    MoveResultCallback = HandlePush
                };
            }

            navigation.MakeMoveRequest(request);
        }

        private Push CurrentPush = null;

        private void HandlePush(MoveRequestResult moveRequestResult)
        {
            if (!_pushPrecessed && Navigation.AreVectorsTheSame(moveRequestResult.PositionToMove, Pos)) {
                _pushPrecessed = true;
                var travelSegmentInfo = new TravelSegmentInfo(this);
                Target = new SimpleNavTarget {Target = travelSegmentInfo.EndPos};
                CurrentPush = null;
                return;
            }
            _pushPrecessed = true;
            
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
            
            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            gameplayManager.HandleGameplayEvent(new ActorTouchedPositionGameplayEvent {
                Actor = Actor,
                Positions = moveRequestResult.TouchedPositions
            });
            
            if (Navigation.AreVectorsTheSame(CurrentPush.Destination, moveRequestResult.PositionToMove)) {
                Target = new SimpleNavTarget {Target = CurrentPush.Destination};
                CurrentPush = null;
            }
        }

        private bool _pushPrecessed = true;
        public void RandomPush()
        {
            var navigation = Navigation.GetNavigationFromContext();
            var map = navigation.Map;
            var snapped = map.SnapPosition(Pos);
            var neighbours = map.GetNeighbours(snapped).ToList();
            int randomIndex = Game.Instance.Random.Next() % neighbours.Count;

            Vector3 randomNeighbour = neighbours[randomIndex];
            CurrentPush = new Push {Destination = randomNeighbour};
            _pushPrecessed = false;
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

            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
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

        public void TurnTo(Vector3 pos)
        {
            _navAgentBehaviour.ActorVisuals.rotation = Quaternion.LookRotation(pos - Pos);
        }
    }
}
