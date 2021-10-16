using Base.Actors;
using Base.Navigation;
using UnityEngine;

namespace Gruntz.UnitController
{
    public struct MoveToPosition : IUnitExecutable
    {
        private class Executable : IUpdatingExecutable
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
            return new Executable { Actor = actor, Target = Target };
        }
    }
}
