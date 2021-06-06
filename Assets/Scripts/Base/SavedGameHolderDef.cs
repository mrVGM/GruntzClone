using System;

namespace Base
{
    public class SavedGameHolderDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new SavedGameHolder();
        }
    }
}
