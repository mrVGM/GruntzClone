using Base.Actors;

namespace Gruntz.UnitController
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
