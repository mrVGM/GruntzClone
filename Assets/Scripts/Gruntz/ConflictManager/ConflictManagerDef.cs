using Base;

namespace Gruntz.ConflictManager
{
    public class ConflictManagerDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new ConflictManager();
        }
    }
}
