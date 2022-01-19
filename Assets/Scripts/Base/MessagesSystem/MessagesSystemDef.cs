namespace Base.MessagesSystem
{
    public class MessagesSystemDef : Def, IRuntimeInstance
    {
        public MessagesSystemExecutionOrderTagDef MessagesSystemExecutionOrderTagDef;
        public IContextObject CreateRuntimeInstance()
        {
            return new MessagesSystem(this);
        }
    }
}
