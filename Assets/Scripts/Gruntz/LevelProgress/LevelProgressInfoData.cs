using Base;
using System;
using System.Collections.Generic;

namespace Gruntz.LevelProgress
{
    [Serializable]
    public class LevelProgressInfoData : ISerializedObjectData
    {
        public List<DefRef<LevelDef>> FinishedLevels = new List<DefRef<LevelDef>>();
    }
}
