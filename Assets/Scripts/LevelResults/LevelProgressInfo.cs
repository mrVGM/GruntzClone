using Base;
using System.Collections.Generic;
using System.Linq;

namespace LevelResults
{
    public class LevelProgressInfo : IContextObject, ISerializedObject
    {
        public LevelProgressInfoDef LevelProgressInfoDef { get; private set; }
        private LevelProgressInfoData _levelProgressInfoData;

        public IEnumerable<LevelDef> FinishedLevels => _levelProgressInfoData.FinishedLevels.Select(x => (LevelDef)x);
        public LevelDef CurrentLevel
        {
            get
            {
                if ((LevelDef)_levelProgressInfoData.CurrentLevel == null) {
                    _levelProgressInfoData.CurrentLevel = LevelProgressInfoDef.InitalLevel.ToDefRef<LevelDef>();
                }
                return _levelProgressInfoData.CurrentLevel;
            }
            set
            {
                _levelProgressInfoData.CurrentLevel = value.ToDefRef<LevelDef>();
            }
        }

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

        public bool IsLevelUnlocked(LevelDef level)
        {
            return LevelProgressInfoDef.UnlockConditions.Where(x => x.Level == level).All(x => x.Satified(FinishedLevels));
        }

        public void DisposeObject()
        {
        }

        public static LevelProgressInfo GetLevelProgressInfoFromContext()
        {
            var game = Game.Instance;
            var levelProgressInfoDef = game.DefRepositoryDef.AllDefs.OfType<LevelProgressInfoDef>().First();
            
            var levelProgressInfo = game.Context.GetRuntimeObject(levelProgressInfoDef) as LevelProgressInfo;
            levelProgressInfo.LevelProgressInfoDef = levelProgressInfoDef;
            return levelProgressInfo;
        }
    }
}
