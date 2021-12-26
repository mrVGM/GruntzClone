using Base.Actors;
using Base.Navigation;
using UnityEngine;

namespace Gruntz.UnitController.Instructions
{
    public struct ClearInstruction : IUnitExecutable
    {
        private class UpdatingExecute : IUpdatingExecutable
        {
            public Actor Actor;
            public void StopExecution()
            {
            }

            public bool UpdateExecutable()
            {
                return false;
            }
        }

        public IUpdatingExecutable Execute(Actor actor)
        {
            return new UpdatingExecute { Actor = actor };
        }
    }
}
