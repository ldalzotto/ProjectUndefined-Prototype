using UnityEngine;
using System.Collections;
using OdinSerializer;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IAbstractTreePickerGUI
{
    void SetSelectedKey(string newSelectedKey);
    bool SetSelectedKey(Type valueType);
    void Init(Action repaintAction);
}

[System.Serializable]
public abstract class AbstractTreePickerGUI<T> : SerializedScriptableObject, IAbstractTreePickerGUI
{
    public abstract Dictionary<string, T> Configurations { get; }

    public Vector2 TreePickerPopupWindowDimensions = new Vector2(300, 400);
    protected TreePickerPopup TreePickerPopup;
    [SerializeField]
    protected string selectedKey;

    protected GUIStyle selectedItemLabelStyle;
    public void Init(Action repaintAction)
    {
        if (this.selectedItemLabelStyle == null)
        {
            this.selectedItemLabelStyle = new GUIStyle(EditorStyles.label);
            this.selectedItemLabelStyle.normal.background = Texture2D.whiteTexture;
        }
        if (this.TreePickerPopup == null)
        {
            var sortedKeys = this.Configurations.Keys.ToList();
            sortedKeys.Sort();
            this.TreePickerPopup = new TreePickerPopup(sortedKeys, OnSelectionChange: this.OnSelectionChange, this.selectedKey);
        }
        this.TreePickerPopup.RepaintAction = repaintAction;
        this.TreePickerPopup.WindowDimensions = this.TreePickerPopupWindowDimensions;
    }

    protected virtual void OnSelectionChange()
    {
        this.selectedKey = this.TreePickerPopup.SelectedKey;
    }

    public T GetSelectedConf()
    {
        if (!string.IsNullOrEmpty(this.selectedKey))
        {
            return this.Configurations[this.selectedKey];
        }
        return default;
    }

    public void SetSelectedKey(string newSelectedKey)
    {
        this.TreePickerPopup.SetSelectedKey(newSelectedKey);
    }

    public bool SetSelectedKey(Type valueType)
    {
        foreach (var configuration in Configurations)
        {
            if (configuration.Key.Contains(valueType.Name))
            {
                this.SetSelectedKey(configuration.Key);
                this.OnSelectionChange();
                return true;
            }
        }
        return false;
    }

}
