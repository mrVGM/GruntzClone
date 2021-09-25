using Gruntz.Status;

namespace Base.Actors
{
    public class ActorManagerDef : Def, IRuntimeInstance
    {
        public StatusDef DeadStatus;
        public IContextObject CreateRuntimeInstance()
        {
            return new ActorManager();
        }
    }
}
