using Gruntz.Actors;
using UnityEngine;

namespace Gruntz.UserInteraction.UnitController
{
    public struct MoveToPosition : IUnitExecutable
    {
        public Vector3 Position;
        void IUnitExecutable.Execute(Actor actor)
        {
            actor.NavAgent.Target = Position;
        }
    }
}
