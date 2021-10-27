using Base.Actors;

namespace Gruntz.UnitController.Instructions
{
    public interface IUpdatingExecutable
    {
        bool UpdateExecutable();
        void StopExecution();
    }
    public interface IUnitExecutable
    {
        IUpdatingExecutable Execute(Actor actor);
    }
}
