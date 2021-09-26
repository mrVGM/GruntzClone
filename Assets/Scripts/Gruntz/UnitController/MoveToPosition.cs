using Base.Actors;
using Base.Navigation;
using UnityEngine;

namespace Gruntz.UnitController
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
