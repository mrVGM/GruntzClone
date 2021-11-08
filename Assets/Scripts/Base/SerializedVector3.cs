using System;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class SerializedVector3
    {
        public float x;
        public float y;
        public float z;

        public static implicit operator Vector3(SerializedVector3 serializedVector)
        {
            return new Vector3(serializedVector.x, serializedVector.y, serializedVector.z);
        }

        public static implicit operator SerializedVector3(Vector3 vector)
        {
            return new SerializedVector3 { x = vector.x, y = vector.y, z = vector.z };
        }
    }
}