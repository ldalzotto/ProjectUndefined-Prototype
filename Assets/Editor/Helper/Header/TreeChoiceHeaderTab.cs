using UnityEngine;
using System.Collections;
using UnityEditor.IMGUI.Controls;
using OdinSerializer;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using System.Linq;
using System;
using System.Text.RegularExpressions;

[System.Serializable]
public abstract class TreeChoiceHeaderTab<T> : AbstractTreePickerGUI<T>
{

    private Rect buttonRect;

    public void GUITick(Action repaintAction)
    {
        Init(repaintAction);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("S", EditorStyles.miniButton, GUILayout.Width(20)))
        {
            PopupWindow.Show(this.buttonRect, this.TreePickerPopup);
        }
        if (!string.IsNullOrEmpty(this.selectedKey))
        {
            var oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            EditorGUILayout.LabelField(this.selectedKey, this.selectedItemLabelStyle);
            GUI.backgroundColor = oldBackgroundColor;
        }
        EditorGUILayout.EndHorizontal();
        if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
    }

 
}
