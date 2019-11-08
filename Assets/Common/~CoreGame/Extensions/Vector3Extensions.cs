using UnityEngine;

namespace CoreGame
{
    public static class Vector3Extensions
    {
        public static Vector3 Add(this Vector3 vector, float x, float y, float z)
        {
            vector.x += x;
            vector.y += y;
            vector.z += z;
            return vector;
        }

        public static Vector3 Add(this Vector3 vector3, Vector3 other)
        {
            return vector3.Add(other.x, other.y, other.z);
        }

        public static Vector3 Mul(this Vector3 vector, float nb)
        {
            vector.x = vector.x * nb;
            vector.y = vector.y * nb;
            vector.z = vector.z * nb;
            return vector;
        }

        public static Vector3 Mul(this Vector3 vector, Vector3 vector2)
        {
            vector.x = vector.x * vector2.x;
            vector.y = vector.y * vector2.y;
            vector.z = vector.z * vector2.z;
            return vector;
        }

        public static Vector3 SetVector(this Vector3 vector, float x, float y, float z)
        {
            vector.x = x;
            vector.y = y;
            vector.z = z;
            return vector;
        }
    }
}