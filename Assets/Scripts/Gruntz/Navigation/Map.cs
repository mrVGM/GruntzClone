using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Navigation
{
    public class Map
    {
        public IEnumerable<Vector3> GetPossibleMoves(Vector3 pos)
        {
            yield return pos;
            yield return pos + Vector3.right;
            yield return pos + Vector3.right + Vector3.forward;
            yield return pos + Vector3.forward;
            yield return pos + Vector3.forward + Vector3.left;
            yield return pos + Vector3.left;
            yield return pos + Vector3.left + Vector3.back;
            yield return pos + Vector3.back;
            yield return pos + Vector3.back + Vector3.right;
        }

        public float MoveCost(Vector3 pos, Vector3 target)
        {
            return (target - pos).sqrMagnitude;
        }
    }
}
