using UnityEditor;
using UnityEngine;

public class HandlesHelper
{

    public static void DrawBoxCollider(BoxCollider boxCollider, Transform transform, Color color, string label, GUIStyle labelStyle)
    {
        DrawBox(boxCollider.center, boxCollider.size, transform, color, label, labelStyle);
    }

    public static void DrawBox(Vector3 center, Vector3 size, Transform transform, Color color, string label, GUIStyle labelStyle)
    {
        var oldColor = Handles.color;
        Handles.color = color;
        Handles.Label(transform.TransformPoint(center + new Vector3(0, size.y * 0.75f, 0)), label, labelStyle);
        var oldMatrix = Handles.matrix;
        Handles.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Handles.DrawWireCube(center, size);
        Handles.matrix = oldMatrix;
        Handles.color = oldColor;
    }

    public static void DrawArrow(Vector3 source, Vector3 target, Color color, float arrowSemiAngle = 25f, float arrowLength = 3f)
    {
        var oldColor = Handles.color;
        Handles.color = color;
        Handles.DrawLine(source, target);
        Handles.DrawLine(target, target + ((Quaternion.AngleAxis(arrowSemiAngle, Vector3.up) * (source - target).normalized) * arrowLength));
        Handles.DrawLine(target, target + ((Quaternion.AngleAxis(-arrowSemiAngle, Vector3.up) * (source - target).normalized) * arrowLength));
        Handles.color = oldColor;
    }
}
