using System;

namespace Base
{
    public class SavedGameHolderDef : IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new SavedGameHolder();
        }
    }
}
