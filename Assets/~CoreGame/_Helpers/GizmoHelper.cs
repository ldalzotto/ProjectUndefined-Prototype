#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

public class GizmoHelper 
{
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