namespace Base.MessagesSystem
{
    public class MessagesSystemDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new MessagesSystem();
        }
    }
}
