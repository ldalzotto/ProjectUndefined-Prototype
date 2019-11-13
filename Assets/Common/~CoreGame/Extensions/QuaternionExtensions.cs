using UnityEngine;

namespace CoreGame
{
    public static class QuaternionExtensions
    {
        public const float QuaternionComparisonRadiantAnglePrecision = 0.001f;

        public static bool IsApproximate(this Quaternion quaternion, Quaternion q2)
        {
            return Mathf.Abs(Quaternion.Dot(quaternion, q2)) >= 1 - QuaternionComparisonRadiantAnglePrecision;
        }
    }
}