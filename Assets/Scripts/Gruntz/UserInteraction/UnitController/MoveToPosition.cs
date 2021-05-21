using Gruntz.Actors;
using Gruntz.Navigation;
using UnityEngine;

namespace Gruntz.UserInteraction.UnitController
{
    public struct MoveToPosition : IUnitExecutable
    {
        public Vector3 Position;
        void IUnitExecutable.Execute(Actor actor)
        {
            actor.GetComponent<NavAgent>().Target = Position;
        }
    }
}
