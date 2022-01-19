namespace Base.Status
{
    public class Status : ISerializedObject
    {
        private StatusDef _statusDef;

        public StatusDef StatusDef
        {
            get
            {
                if (_statusDef == null) {
                    _statusDef = StatusData.StatusDef;
                }
                return _statusDef;
            }
        }
        private StatusData _statusData;
        public StatusData StatusData => _statusData;
        public ISerializedObjectData Data
        {
            get => _statusData;
            set => _statusData = value as StatusData;
        }
    }
}
