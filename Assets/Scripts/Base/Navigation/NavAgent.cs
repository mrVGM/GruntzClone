using System;
using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.Gameplay;
using UnityEngine;

namespace Base.Navigation
{
    public class NavAgent : IActorComponent, IOrderedUpdate, ISerializedObject
    {
        public enum NavAgentState
        {
            Moving,
            Statying,
            BeingPushed
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

        public INavigationTarget StopASAPNavTarget => new SimpleNavTarget { Target = TravelSegment.EndPos };

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
        public ITravelSegmentInfo TravelSegment
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
        
        public INavAgentController Controller
        {
            get
            {
                if (_navAgentData.NavAgentController == null) {
                    _navAgentData.NavAgentController = new RegularMoveNavAgentController(this);
                }

                _navAgentData.NavAgentController.NavAgent = this;
                return _navAgentData.NavAgentController;
            }
            set
            {
                _navAgentData.NavAgentController = value;
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
            navigation.MakeMoveRequest(Controller.MoveRequest);
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

        public void SetTravelSegmentAndLocation(Vector3 pos, Vector3 dir, ITravelSegmentInfo travelSegment)
        {
            _navAgentBehaviour.ActorVisuals.position = pos;
            _navAgentBehaviour.NavObstacle.transform.position = travelSegment.EndPos;
            _navAgentBehaviour.LocalTravelStartPoint.position = travelSegment.StartPos;

            if (!Navigation.AreVectorsTheSame(dir, Vector3.zero)) {
                _navAgentBehaviour.ActorVisuals.rotation = Quaternion.LookRotation(dir);
            }
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
