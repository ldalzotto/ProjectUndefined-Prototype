using System;
using UnityEditor;
using UnityEngine;

public class FoldableArea
{

    private bool isFolded;
    private string foldContent;

    private bool canBeDisabled;
    private bool isEnabled;
   
    public FoldableArea(bool canBeDisabled, string foldContent, bool isEnabledStartValue)
    {
        this.canBeDisabled = canBeDisabled;
        this.foldContent = foldContent;
        this.isEnabled = isEnabledStartValue;
    }

    public bool IsEnabled { get => isEnabled; }

    public void Enable() { this.isEnabled = true; }
    public void Disable() { this.isEnabled = false; }

    public bool OnGUI(Action guiAction)
    {
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        if (this.canBeDisabled)
        {
            EditorGUILayout.BeginHorizontal();
            this.isEnabled = EditorGUILayout.Toggle(this.isEnabled, GUILayout.Width(30f));
            this.isFolded = EditorGUILayout.Foldout(this.isFolded, this.foldContent, true);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            this.isFolded = EditorGUILayout.Foldout(this.isFolded, this.foldContent, true);
        }

        EditorGUI.BeginDisabledGroup(this.canBeDisabled && !this.isEnabled);
        if (this.isFolded)
        {
            guiAction.Invoke();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();

        return this.isEnabled;
    }
}
