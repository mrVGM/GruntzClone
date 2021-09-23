using Base;

namespace Gruntz.Puzzle
{
    public class GameplayManagerDef : Def, IRuntimeInstance
    {
        public GameplayActionGenerator[] GameplayActionGenerators;
        public IContextObject CreateRuntimeInstance()
        {
            return new GameplayManager(this);
        }
    }
}
