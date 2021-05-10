using Base;

namespace Gruntz.Actors
{
    public class ActorManagerDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new ActorManager();
        }
    }
}
