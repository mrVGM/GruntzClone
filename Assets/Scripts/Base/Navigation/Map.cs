using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Navigation
{
    public class Map
    {
        private IEnumerable<Vector3> GetPotentialMoves(Vector3 pos)
        {
            yield return pos + Vector3.back;
            yield return pos + Vector3.right;
            yield return pos + Vector3.right + Vector3.forward;
            yield return pos + Vector3.forward;
            yield return pos + Vector3.forward + Vector3.left;
            yield return pos + Vector3.left;
            yield return pos + Vector3.left + Vector3.back;
            yield return pos + Vector3.back + Vector3.right;
        }

        
        public IEnumerable<Vector3> GetPossibleMoves(Vector3 pos, LayerMask layerMask)
        {
            yield return pos;

            var potentialMoves = GetPotentialMoves(pos);

            foreach (var potentialMove in potentialMoves)
            {
                var offset = potentialMove - pos;
                var ray = new Ray(pos, offset);
                IEnumerable<RaycastHit> hits = Physics.SphereCastAll(ray, .2f, offset.magnitude, layerMask);
                hits = hits.Where(x => !x.collider.bounds.Contains(pos));
                if (hits.Any())
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

        public Vector3 SnapPosition(Vector3 vector)
        {
            Vector3 center = 0.5f * (Vector3.right + Vector3.forward);
            return new Vector3(Mathf.Floor(vector.x), 0, Mathf.Floor(vector.z)) + center;
        }

        public IEnumerable<Vector3> GetNeighbours(Vector3 vector)
        {
            return GetPotentialMoves(vector);
        }
    }
}
