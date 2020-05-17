using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Foldable))]
public class FoldablePropertyDrawer : PropertyDrawer
{
    private FoldableArea foldableArea;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Foldable foldableAttribute = (Foldable)attribute;

        SerializedProperty disablingProperty = null;
        if (foldableAttribute.CanBeDisabled)
        {
            var parentProperty = SerializableObjectHelper.GetParentProperty(property);
            disablingProperty = parentProperty.FindPropertyRelative(foldableAttribute.DisablingBoolAttribute);
            if (this.foldableArea == null)
            {
                this.foldableArea = new FoldableArea(foldableAttribute.CanBeDisabled, property.name, disablingProperty.boolValue);
            }
        }
        else
        {
            if (this.foldableArea == null)
            {
                this.foldableArea = new FoldableArea(foldableAttribute.CanBeDisabled, property.name, false);
            }
        }


        EditorGUI.BeginProperty(position, null, property);
        this.foldableArea.OnGUI(() =>
        {
            try
            {
                foreach (var childPropery in SerializableObjectHelper.GetChildren(property))
                {

                    EditorGUILayout.PropertyField(childPropery, true);
                }
            }
            catch (Exception) { }
        });

        if (foldableAttribute.CanBeDisabled)
        {
            disablingProperty.boolValue = this.foldableArea.IsEnabled;
        }
        EditorGUI.EndProperty();
    }


}
