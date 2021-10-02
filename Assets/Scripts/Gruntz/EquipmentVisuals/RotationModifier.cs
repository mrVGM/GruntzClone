using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.EquipmentVisuals
{
    public class RotationModifier : MonoBehaviour
    {
        [Serializable]
        public class PointMap
        {
            public Transform P1;
            public Transform P2;
        }

        public PointMap Origin;
        public PointMap Target;

        private void Update()
        {
            Vector3 realVector = Target.P1.position - Origin.P1.position;
            Vector3 refVector = Target.P2.position - Origin.P2.position;

            Vector3 realPole = Vector3.Cross(realVector, Vector3.up);
            Vector3 refPole = Vector3.Cross(refVector, Vector3.up);

            var rot1 = Quaternion.FromToRotation(realPole, refPole);
            Vector3 realRotated = rot1 * realVector;
            var rot2 = Quaternion.FromToRotation(realRotated, refVector);

            transform.position = Origin.P2.position;
            transform.rotation = rot2 * rot1 * transform.rotation;
        }
    }
}
