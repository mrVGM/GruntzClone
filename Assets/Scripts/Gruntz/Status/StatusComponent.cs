using Base;
using Gruntz.Actors;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Status
{
    public class StatusComponent : IActorComponent, ISerializedObject
    {
        private List<Status> _statuses = new List<Status>();

        public ISerializedObjectData Data
        {
            get => new StatusComponentData { StatusDatas = _statuses.Select(x => x.Data as StatusData).ToList() };
            set
            {
                _statuses.Clear();
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
        }

        public void RemoveStatus(Status status)
        {
            _statuses.Remove(status);
        }
    }
}
