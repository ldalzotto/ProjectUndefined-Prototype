using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;

[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
public class ReorderableListPropertyDrawer : PropertyDrawer
{
    private ReorderableList rList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ReorderableListAttribute attr = (ReorderableListAttribute)attribute;
        var parentProperty = SerializableObjectHelper.GetParentProperty(property);

        position.height *= 3;
        if (parentProperty != null && parentProperty.isArray)
        {
            if (SerializableObjectHelper.GetArrayIndex(property) == 0)
            {
                if (this.rList == null)
                {
                    this.rList = new ReorderableList(property.serializedObject, parentProperty);
                    this.rList.drawHeaderCallback = (Rect rect) =>
                    {
                        EditorGUI.LabelField(rect, SerializableObjectHelper.GetParentProperty(parentProperty).name);
                    };
                    this.rList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        rect.y += 2;
                        rect.height -= 5;
                        if (attr.DisplayLineLabel)
                        {
                            EditorGUI.PropertyField(rect, this.rList.serializedProperty.GetArrayElementAtIndex(index), true);
                        }
                        else
                        {
                            EditorGUI.PropertyField(rect, this.rList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
                        }
                    };
                    this.rList.elementHeightCallback = (int index) =>
                    {
                        return EditorGUI.GetPropertyHeight(this.rList.serializedProperty.GetArrayElementAtIndex(index)) + 3;
                    };
                }
                this.rList.DoList(position);
            }

        }

        parentProperty.serializedObject.ApplyModifiedProperties();

    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (SerializableObjectHelper.GetArrayIndex(property) == 0)
        {
            if (this.rList != null)
            {

                return this.rList.GetHeight();
            }
            return base.GetPropertyHeight(property, label);
        }
        else
        {
            return 0f;
        }

    }
}
