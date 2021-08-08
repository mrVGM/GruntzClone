using Base;

namespace Gruntz.Puzzle
{
    public class GameplayManagerDef : Def, IRuntimeInstance
    {
        public GameplayEventHandlerDef[] Handlers;

        public IContextObject CreateRuntimeInstance()
        {
            return new GameplayManager(this);
        }
    }
}
