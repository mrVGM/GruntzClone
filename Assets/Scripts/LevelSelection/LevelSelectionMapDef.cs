using Base;

namespace LevelSelection
{
    public class LevelSelectionMapDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new LevelSelectionMap();
        }
    }
}
