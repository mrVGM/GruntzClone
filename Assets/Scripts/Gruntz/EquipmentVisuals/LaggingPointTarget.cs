using Base;
using UnityEngine;

namespace Gruntz.EquipmentVisuals
{
    public class LaggingPointTarget : MonoBehaviour
    {
        public TagDef TagDef;
        public float Lag;
        public Transform FixedDistance;
        public float Threshold = 0.001f;

        public bool LaggingEnabled = true;

        public Vector3 GetPosition(LaggingPoint laggingPoint)
        {
            if (!LaggingEnabled) {
                return transform.position;
            }

            if ((laggingPoint.transform.position - transform.position).magnitude < Threshold) {
                return transform.position;
            }

            var c = Mathf.Clamp01(Time.deltaTime * Lag);
            Vector3 calculatedPoint = (1 - c) * laggingPoint.transform.position + c * transform.position;

            if (FixedDistance == null) {
                return calculatedPoint;
            }

            Vector3 fixedOffset = FixedDistance.position - transform.position;
            Vector3 offset = calculatedPoint - FixedDistance.position;
            Vector3 step = calculatedPoint - laggingPoint.transform.position;
            float dist = fixedOffset.magnitude;

            if (offset.magnitude > dist) {
                return calculatedPoint;
            }

            Vector3 tangent = step.magnitude * Vector3.Cross(Vector3.up, offset).normalized;
            if (Vector3.Dot(-tangent, fixedOffset) < Vector3.Dot(tangent, fixedOffset)) {
                tangent = -tangent;
            }
            Vector3 newPoint = step + tangent;
            
            newPoint = step.magnitude * newPoint.normalized + laggingPoint.transform.position;
            return newPoint;
        }
    }
}
