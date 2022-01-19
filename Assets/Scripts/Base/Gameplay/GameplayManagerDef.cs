using Base;

namespace Base.Gameplay
{
    public class GameplayManagerDef : Def, IRuntimeInstance
    {
        public GameplayManagerExecutionOrderTag GameplayManagerExecutionOrderTag;
        public GameplayActionsProcessorDef gameplayActionsProcessorDef;
        public IContextObject CreateRuntimeInstance()
        {
            return new GameplayManager(this);
        }
    }
}
