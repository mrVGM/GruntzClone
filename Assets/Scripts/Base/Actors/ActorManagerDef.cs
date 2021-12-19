using Base.Status;

namespace Base.Actors
{
    public class ActorManagerDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new ActorManager();
        }
    }
}
