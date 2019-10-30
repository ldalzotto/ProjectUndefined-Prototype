using UnityEngine;

namespace CoreGame
{
    public static class QuaterionHelper
    {
        public static bool ApproxEquals(Quaternion a, Quaternion b)
        {
            return Mathf.Abs(Quaternion.Angle(a, b)) <= 0.1f;
        }
    }

}
