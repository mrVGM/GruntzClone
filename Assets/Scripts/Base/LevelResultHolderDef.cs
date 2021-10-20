namespace Base
{
    public class LevelResultHolderDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new LevelResultHolder();
        }
    }
}
