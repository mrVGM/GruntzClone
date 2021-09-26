namespace Base.Navigation
{
    public class NavigationDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance() 
        {
            return new Navigation();
        }
    }
}
