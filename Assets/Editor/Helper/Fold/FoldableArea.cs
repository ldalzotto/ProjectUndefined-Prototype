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

    public bool IsEnabled
    {
        get => isEnabled;
    }

    public void Enable()
    {
        this.isEnabled = true;
    }

    public void Disable()
    {
        this.isEnabled = false;
    }

    public bool OnGUI(Action guiAction)
    {
        var foldContentText = "";
        for (var i = 0; i < EditorGUI.indentLevel; i++)
        {
            foldContentText += " ";
        }

        foldContentText += this.foldContent;
        if (this.canBeDisabled)
        {
            this.isFolded = EditorGUILayout.BeginFoldoutHeaderGroup(this.isFolded, foldContentText, null, this.ShowContextMenu);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        else
        {
            this.isFolded = EditorGUILayout.BeginFoldoutHeaderGroup(this.isFolded, foldContentText);
            EditorGUILayout.EndFoldoutHeaderGroup(); 
        }
       
        EditorGUI.BeginDisabledGroup(this.canBeDisabled && !this.isEnabled);
        if (this.isFolded)
        {
            guiAction.Invoke();
        }

        EditorGUI.EndDisabledGroup();
        return this.isEnabled;
    }

    private void ShowContextMenu(Rect position)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Enabled"), this.isEnabled, () => { this.isEnabled = !this.isEnabled;} );
        menu.DropDown(position);
    }
}