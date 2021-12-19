namespace Base.UI
{
    public class BrainDef : Def, IRuntimeInstance
    {
        public ExecutionOrderTagDef ExecutionOrderTagDef;
        public IContextObject CreateRuntimeInstance()
        {
            return new Brain(this);
        }
    }
}
