using Base.Actors;
using Gruntz.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Status
{
    public class StatusComponent : IActorComponent, ISerializedObject
    {
        public class StatusGameplayEvent : GameplayEvent
        {
            public enum Operation
            {
                Added,
                Removed
            }
            public Operation OperationExecuted;
            public Actor Actor;
            public Status Status;
        }

        private Actor _actor;
        private List<Status> _statuses = new List<Status>();

        public StatusComponent(Actor actor)
        {
            _actor = actor;
        }

        public ISerializedObjectData Data
        {
            get => new StatusComponentData { StatusDatas = _statuses.Select(x => x.Data as StatusData).ToList() };
            set
            {
                _statuses.Clear();
                while (_statuses.Any())
                {
                    var status = _statuses.First();
                    RemoveStatus(status);
                }
                var statusComponentData = value as StatusComponentData;
                foreach (var statusData in statusComponentData.StatusDatas)
                {
                    var status = statusData.CreateStatus();
                    AddStatus(status);
                }
            }
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }

        public void AddStatus(Status status)
        {
            _statuses.Add(status);
            var gameplayManager = GameplayManager.GetActorManagerFromContext();

            gameplayManager.HandleGameplayEvent(new StatusGameplayEvent { 
                OperationExecuted = StatusGameplayEvent.Operation.Added,
                Actor = _actor,
                Status = status
            });
        }

        public void RemoveStatus(Status status)
        {
            _statuses.Remove(status);

            var gameplayManager = GameplayManager.GetActorManagerFromContext();
            gameplayManager.HandleGameplayEvent(new StatusGameplayEvent
            {
                OperationExecuted = StatusGameplayEvent.Operation.Removed,
                Actor = _actor,
                Status = status
            });
        }

        public Status GetStatus(StatusDef statusDef)
        {
            return _statuses.FirstOrDefault(x => x.StatusDef == statusDef);
        }

        public IEnumerable<Status> GetStatuses(Func<Status, bool> predicate)
        {
            return _statuses.Where(predicate);
        }
    }
}
