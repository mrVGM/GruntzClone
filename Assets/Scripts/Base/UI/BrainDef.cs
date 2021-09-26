namespace Base.UI
{
    public class BrainDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new Brain();
        }
    }
}
