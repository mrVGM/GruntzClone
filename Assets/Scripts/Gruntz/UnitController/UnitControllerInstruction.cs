using Base.Actors;

namespace Gruntz.UnitController
{
    public struct UnitControllerInstruction
    {
        public Actor Unit;
        public IUnitExecutable Executable;
    }
}
