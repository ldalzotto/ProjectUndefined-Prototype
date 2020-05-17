using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(MyReadOnly))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property);
        EditorGUI.EndDisabledGroup();
    }
}
