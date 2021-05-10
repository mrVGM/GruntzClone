using Gruntz.Actors;

namespace Gruntz.UserInteraction.UnitController
{
    public struct UnitControllerInstruction
    {
        public Actor Unit;
        public IUnitExecutable Executable;
    }
}
