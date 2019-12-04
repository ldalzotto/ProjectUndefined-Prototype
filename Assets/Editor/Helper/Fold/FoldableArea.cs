using System;
using UnityEditor;
using UnityEngine;

public class FoldableArea
{
    private IFoldableAreaFoldoutValueStrategy IFoldableAreaFoldoutValueStrategy;
    private string foldContent;

    private bool canBeDisabled;
    private bool isEnabled;

    public FoldableArea(bool canBeDisabled, string foldContent, bool isEnabledStartValue, EditorPersistantBoolVariable isFoldedPersistedBoolVariable = null)
    {
        this.canBeDisabled = canBeDisabled;
        this.foldContent = foldContent;
        this.isEnabled = isEnabledStartValue;
        if (isFoldedPersistedBoolVariable == null)
        {
            this.IFoldableAreaFoldoutValueStrategy = new NonPersistedFoldableArea();
        }
        else
        {
            this.IFoldableAreaFoldoutValueStrategy = new FoldableAreaPersistance(isFoldedPersistedBoolVariable);
        }
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
            this.IFoldableAreaFoldoutValueStrategy.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.IFoldableAreaFoldoutValueStrategy.GetValue(), foldContentText, null, this.ShowContextMenu));
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        else
        {
            this.IFoldableAreaFoldoutValueStrategy.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.IFoldableAreaFoldoutValueStrategy.GetValue(), foldContentText));
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        EditorGUI.BeginDisabledGroup(this.canBeDisabled && !this.isEnabled);
        if (this.IFoldableAreaFoldoutValueStrategy.GetValue())
        {
            guiAction.Invoke();
        }

        EditorGUI.EndDisabledGroup();
        return this.isEnabled;
    }

    private void ShowContextMenu(Rect position)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Enabled"), this.isEnabled, () => { this.isEnabled = !this.isEnabled; });
        menu.DropDown(position);
    }
}

interface IFoldableAreaFoldoutValueStrategy
{
    bool GetValue();
    void SetValue(bool value);
}

class FoldableAreaPersistance : IFoldableAreaFoldoutValueStrategy
{
    private EditorPersistantBoolVariable isFoldedPersisted;

    public FoldableAreaPersistance(EditorPersistantBoolVariable isFoldedPersisted)
    {
        this.isFoldedPersisted = isFoldedPersisted;
    }

    public bool GetValue()
    {
        return this.isFoldedPersisted.GetValue();
    }

    public void SetValue(bool value)
    {
        this.isFoldedPersisted.SetValue(value);
    }
}

class NonPersistedFoldableArea : IFoldableAreaFoldoutValueStrategy
{
    private bool value;

    public bool GetValue()
    {
        return this.value;
    }

    public void SetValue(bool value)
    {
        this.value = value;
    }
}