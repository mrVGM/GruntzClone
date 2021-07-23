using Base;
using Gruntz.Actors;

namespace Gruntz.Status
{
    public class Status : ISerializedObject
    {
        public StatusDef StatusDef => _statusData.StatusDef;
        private StatusData _statusData;
        public ISerializedObjectData Data
        {
            get => _statusData;
            set => _statusData = value as StatusData;
        }
    }
}
