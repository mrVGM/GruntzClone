using Base.Actors;
using Gruntz.UnitController.Instructions;

namespace Gruntz.UnitController
{
    public struct UnitControllerInstruction
    {
        public Actor Unit;
        public IUnitExecutable Executable;
    }
}
