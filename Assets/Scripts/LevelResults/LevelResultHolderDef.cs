using Base;

namespace LevelResults
{
    public class LevelResultHolderDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new LevelResultHolder();
        }
    }
}
