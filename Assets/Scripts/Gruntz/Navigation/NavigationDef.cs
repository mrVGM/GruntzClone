using Base;

namespace Gruntz.Navigation
{
    public class NavigationDef : Def, IRuntimeInstance
    {
        public object CreateRuntimeInstance() 
        {
            return new Navigation();
        }
    }
}
