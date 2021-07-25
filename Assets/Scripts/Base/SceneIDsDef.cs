using System;

namespace Base
{
    public class SceneIDsDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new SceneIDsHolder();
        }
    }
}
