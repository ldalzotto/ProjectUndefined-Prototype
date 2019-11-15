using UnityEngine;
using UnityEngine.Profiling;

namespace CoreGame
{
    public static class BoundsExtensions
    {
        public static Rect ToScreenSpace(this Bounds bounds, Camera camera)
        {
            Vector2 center = camera.WorldToScreenPoint(bounds.center);

            Vector2 P1 = camera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z));
            Vector2 P2 = camera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z));
            Vector2 P3 = camera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z));
            Vector2 P4 = camera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z));


            Vector2 P5 = camera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z));
            Vector2 P6 = camera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z));
            Vector2 P7 = camera.WorldToScreenPoint(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z));
            Vector2 P8 = camera.WorldToScreenPoint(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z));

            float width = MaxOperation(P1.x, P2.x, P3.x, P4.x, P5.x, P6.x, P7.x, P8.x) - MinOperation(P1.x, P2.x, P3.x, P4.x, P5.x, P6.x, P7.x, P8.x);
            float height = MaxOperation(P1.y, P2.y, P3.y, P4.y, P5.y, P6.y, P7.y, P8.y) - MinOperation(P1.y, P2.y, P3.y, P4.y, P5.y, P6.y, P7.y, P8.y);

            return new Rect(center - new Vector2(width * 0.5f, height * 0.5f), new Vector2(width, height));
        }

        private static float MaxOperation(float p1, float p2, float p3, float p4, float p5, float p6, float p7, float p8)
        {
            return Mathf.Max(p1, Mathf.Max(p2, Mathf.Max(p3, Mathf.Max(p4, Mathf.Max(p5, Mathf.Max(p6, Mathf.Max(p7, p8)))))));
        }

        private static float MinOperation(float p1, float p2, float p3, float p4, float p5, float p6, float p7, float p8)
        {
            return Mathf.Min(p1, Mathf.Min(p2, Mathf.Min(p3, Mathf.Min(p4, Mathf.Min(p5, Mathf.Min(p6, Mathf.Min(p7, p8)))))));
        }

        public static Bounds Mul(this Bounds bounds, Matrix4x4 matrix4X4)
        {
            return new Bounds(matrix4X4.MultiplyPoint(bounds.center), matrix4X4.lossyScale.Mul(bounds.size));
        }
    }
}