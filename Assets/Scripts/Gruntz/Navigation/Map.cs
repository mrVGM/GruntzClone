using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Gruntz.Navigation
{
    public class Map
    {
        private IEnumerable<Vector3> GetPotentialMoves(Vector3 pos)
        {
            yield return pos + Vector3.right;
            yield return pos + Vector3.right + Vector3.forward;
            yield return pos + Vector3.forward;
            yield return pos + Vector3.forward + Vector3.left;
            yield return pos + Vector3.left;
            yield return pos + Vector3.left + Vector3.back;
            yield return pos + Vector3.back;
            yield return pos + Vector3.back + Vector3.right;
        }

        public IEnumerable<Vector3> GetPossibleMoves(Vector3 pos, LayerMask layerMask)
        {
            yield return pos;

            var potentialMoves = GetPotentialMoves(pos);

            foreach (var potentialMove in potentialMoves)
            {
                var ray = new Ray(potentialMove + Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, 1, layerMask))
                {
                    continue;
                }
                yield return potentialMove;
            }
        }

        public float MoveCost(Vector3 pos, Vector3 target)
        {
            return (target - pos).sqrMagnitude;
        }
    }
}
