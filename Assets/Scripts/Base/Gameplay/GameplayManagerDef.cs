using Base;

namespace Base.Gameplay
{
    public class GameplayManagerDef : Def, IRuntimeInstance
    {
        public GameplayActionGenerator[] GameplayActionGenerators;
        public GameplayActionsProcessorDef gameplayActionsProcessorDef;
        public IContextObject CreateRuntimeInstance()
        {
            return new GameplayManager(this);
        }
    }
}
