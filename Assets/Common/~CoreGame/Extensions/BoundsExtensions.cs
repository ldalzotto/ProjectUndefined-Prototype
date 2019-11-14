using UnityEngine;

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

            float width = Mathf.Max(P1.x, P2.x, P3.x, P4.x, P5.x, P6.x, P7.x, P8.x) - Mathf.Min(P1.x, P2.x, P3.x, P4.x, P5.x, P6.x, P7.x, P8.x);
            float height = Mathf.Max(P1.y, P2.y, P3.y, P4.y, P5.y, P6.y, P7.y, P8.y) - Mathf.Min(P1.y, P2.y, P3.y, P4.y, P5.y, P6.y, P7.y, P8.y);

            return new Rect(center - new Vector2(width*0.5f, height*0.5f), new Vector2(width, height));
        }

        public static Bounds Mul(this Bounds bounds, Matrix4x4 matrix4X4)
        {
            return new Bounds(matrix4X4.MultiplyPoint(bounds.center), matrix4X4.lossyScale.Mul(bounds.size));
        }
    }
}