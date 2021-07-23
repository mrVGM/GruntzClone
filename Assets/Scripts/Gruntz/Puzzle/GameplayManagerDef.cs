using Base;

namespace Gruntz.Puzzle
{
    public class GameplayManagerDef : Def, IRuntimeInstance
    {
        public IContextObject CreateRuntimeInstance()
        {
            return new GameplayManager();
        }
    }
}
