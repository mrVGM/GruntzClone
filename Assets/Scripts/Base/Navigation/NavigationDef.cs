namespace Base.Navigation
{
    public class NavigationDef : Def, IRuntimeInstance
    {
        public NavigationExecutionOrderTagDef NavigationExecutionOrderTagDef;
        public IContextObject CreateRuntimeInstance() 
        {
            return new Navigation(this);
        }
    }
}
