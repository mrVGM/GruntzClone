using Base;

namespace Gruntz.CollectedMaterialManager
{
    public class CollectedMaterialManagerDef : Def, IRuntimeInstance
    {
        public int MaterialNeededForFullUnit = 4;
        public IContextObject CreateRuntimeInstance()
        {
            return new CollectedMaterialManager();
        }
    }
}
