using Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LevelResults
{
    public class LevelProgressInfoDef : Def, IRuntimeInstance
    {
        [Serializable]
        public class UnlockCondition
        {
            public LevelDef Level;
            public LevelDef[] CompletedLevels;
            public bool Satified(IEnumerable<LevelDef> completedLevels)
            {
                return !CompletedLevels.Except(completedLevels).Any();
            }
        }
        public LevelDef InitalLevel;
        public UnlockCondition[] UnlockConditions;

        public IContextObject CreateRuntimeInstance()
        {
            return new LevelProgressInfo();
        }
    }
}
