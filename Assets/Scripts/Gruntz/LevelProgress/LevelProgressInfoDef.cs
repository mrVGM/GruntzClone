using Base;

namespace Gruntz.LevelProgress
{
    public class LevelProgressInfoDef : Def, IRuntimeInstance
    {
        public LevelDef[] AllLevels;
        public IContextObject CreateRuntimeInstance()
        {
            return new LevelProgressInfo();
        }
    }
}
