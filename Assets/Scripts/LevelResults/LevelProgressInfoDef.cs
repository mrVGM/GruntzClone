using Base;

namespace LevelResults
{
    public class LevelProgressInfoDef : Def, IRuntimeInstance
    {
        public LevelDef InitalLevel;
        public LevelDef[] AllLevels;
        public IContextObject CreateRuntimeInstance()
        {
            return new LevelProgressInfo();
        }
    }
}
