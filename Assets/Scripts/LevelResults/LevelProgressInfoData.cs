using Base;
using System;
using System.Collections.Generic;

namespace LevelResults
{
    [Serializable]
    public class LevelProgressInfoData : ISerializedObjectData
    {
        public DefRef<LevelDef> CurrentLevel;
        public List<DefRef<LevelDef>> FinishedLevels = new List<DefRef<LevelDef>>();
    }
}
