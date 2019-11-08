#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class GizmoHelper
{
    public static void DrawBox(Matrix4x4 LocalToWorld, Vector3 LocalCenter, Bounds bounds, Color color, string label, GUIStyle labelStyle)
    {
        var oldColor = Gizmos.color;
        Gizmos.color = color;
        var worldCenter = LocalToWorld.MultiplyPoint(LocalCenter);
        Handles.Label(new Vector3(worldCenter.x, worldCenter.y + bounds.max.y + 10f, worldCenter.z), label, labelStyle);
        Gizmos.DrawWireCube(worldCenter, bounds.size);
        Gizmos.color = oldColor;
    }

    public static void DrawBoxCollider(BoxCollider boxCollider, Transform transform, Color color, string label, GUIStyle labelStyle)
    {
        var oldColor = Gizmos.color;
        Gizmos.color = color;
        Handles.Label(transform.TransformPoint(new Vector3(boxCollider.center.x, boxCollider.center.y + boxCollider.bounds.max.y + 10f, boxCollider.center.z)), label, labelStyle);
        Gizmos.DrawWireCube(transform.TransformPoint(boxCollider.center), boxCollider.size);
        Gizmos.color = oldColor;
    }
}
#endif