using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Foldable))]
public class FoldablePropertyDrawer : PropertyDrawer
{
    private FoldableArea foldableArea;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return -base.GetPropertyHeight(property, label) * 0.15f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Foldable foldableAttribute = (Foldable) attribute;

        SerializedProperty disablingProperty = null;
        if (foldableAttribute.CanBeDisabled)
        {
            var parentProperty = SerializableObjectHelper.GetParentProperty(property);
            if (parentProperty.name != property.name)
            {
                disablingProperty = parentProperty.FindPropertyRelative(foldableAttribute.DisablingBoolAttribute);
            }
            else
            {
                disablingProperty = property.serializedObject.FindProperty(foldableAttribute.DisablingBoolAttribute);
            }

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
                var childrens = SerializableObjectHelper.GetChildren(property);
                if (childrens != null && childrens.Count > 0)
                {
                    foreach (var childPropery in SerializableObjectHelper.GetChildrenIterable(property))
                    {
                        EditorGUILayout.PropertyField(childPropery, true);
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(property, true);
                }

                EditorGUILayout.Space();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });

        if (foldableAttribute.CanBeDisabled)
        {
            disablingProperty.boolValue = this.foldableArea.IsEnabled;
        }

        EditorGUI.EndProperty();
    }
}