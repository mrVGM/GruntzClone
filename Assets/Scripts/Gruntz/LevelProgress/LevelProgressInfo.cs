using Base;

namespace Gruntz.LevelProgress
{
    public class LevelProgressInfo : IContextObject, ISerializedObject
    {
        private LevelProgressInfoData _levelProgressInfoData;

        public ISerializedObjectData Data
        {
            get
            {
                if (_levelProgressInfoData == null) {
                    _levelProgressInfoData = new LevelProgressInfoData();
                }
                return _levelProgressInfoData;
            }
            set
            {
                _levelProgressInfoData = value as LevelProgressInfoData;
            }
        }

        public void DisposeObject()
        {
        }
    }
}
