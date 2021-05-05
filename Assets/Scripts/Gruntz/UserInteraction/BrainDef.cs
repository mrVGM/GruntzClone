using Base;

namespace Gruntz.UserInteraction
{
    public class BrainDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new Brain();
        }
    }
}
