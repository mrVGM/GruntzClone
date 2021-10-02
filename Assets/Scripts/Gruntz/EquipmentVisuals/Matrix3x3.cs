using UnityEngine;

namespace Gruntz.EquipmentVisuals
{
    public struct Matrix3x3
    {
        public float[,] Elements;

        public Matrix3x3(Vector3 v1, Vector3 v2)
        {
            Elements = new float[3, 3];
            v1.Normalize();

            Elements[0, 0] = v1.x;
            Elements[0, 1] = v1.y;
            Elements[0, 2] = v1.z;

            Vector3 pole = Vector3.Cross(v1, v2).normalized;
            Elements[1, 0] = pole.x;
            Elements[1, 1] = pole.y;
            Elements[1, 2] = pole.z;

            Vector3 other = Vector3.Cross(pole, v1);
            Elements[2, 0] = other.x;
            Elements[2, 1] = other.y;
            Elements[2, 2] = other.z;
        }

        public Matrix3x3(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Elements = new float[3, 3];
            Elements[0, 0] = v1.x;
            Elements[0, 1] = v1.y;
            Elements[0, 2] = v1.z;

            Elements[1, 0] = v2.x;
            Elements[1, 1] = v2.y;
            Elements[1, 2] = v2.z;

            Elements[2, 0] = v3.x;
            Elements[2, 1] = v3.y;
            Elements[2, 2] = v3.z;
        }

        public Vector3 GetRow(int index)
        {
            return new Vector3(Elements[index, 0], Elements[index, 1], Elements[index, 2]);
        }
        public Vector3 GetColumn(int index)
        {
            return new Vector3(Elements[0, index], Elements[1, index], Elements[2, index]);
        }

        public Matrix3x3 Inverse()
        {
            return new Matrix3x3(GetColumn(0), GetColumn(1), GetColumn(2));
        }

        public Vector3 Transform(Vector3 v)
        {
            return v.x * GetColumn(0) + v.y * GetColumn(1) + v.z * GetColumn(2);
        }
    }
}
