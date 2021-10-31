using Base;

namespace Gruntz.CollectedMaterialManager
{
    public class CollectedMaterialManagerDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new CollectedMaterialManager();
        }
    }
}
