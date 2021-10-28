using Base.Actors;
using Base.Navigation;
using UnityEngine;

namespace Gruntz.UnitController.Instructions
{
    public struct MoveToPosition : IUnitExecutable
    {
        private class UpdatingExecute : IUpdatingExecutable
        {
            public Actor Actor;
            public INavigationTarget Target;
            public void StopExecution()
            {
            }

            public bool UpdateExecutable()
            {
                var navAgent = Actor.GetComponent<NavAgent>();
                navAgent.Target = Target;
                return false;
            }
        }

        public INavigationTarget Target;

        public IUpdatingExecutable Execute(Actor actor)
        {
            return new UpdatingExecute { Actor = actor, Target = Target };
        }
    }
}
