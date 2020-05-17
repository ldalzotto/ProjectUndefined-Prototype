using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

[System.Serializable]
public class ButtonTreePickerGUI 
{
    protected TreePickerPopup selectionPicker;
    protected List<string> availableChoices;
    protected Rect enumChoiceDropdownButton;

    private Action OnEnumSelectedEvent;
    
    public Rect EnumChoiceDropdownButton { get => enumChoiceDropdownButton; }
    public List<string> AvailableChoices { get => availableChoices;  }

    public ButtonTreePickerGUI() { }

    public ButtonTreePickerGUI(List<string> keys, Action OnEnumSelectedEvent)
    {
        this.BaseInit(keys, OnEnumSelectedEvent);
    }

    public void BaseInit(List<string> keys, Action OnEnumSelectedEvent)
    {
        this.OnEnumSelectedEvent = OnEnumSelectedEvent;
        this.availableChoices = keys;
        this.selectionPicker = new TreePickerPopup(this.availableChoices, this.OnEnumSelectedEvent, string.Empty);
    }
    
    public void OnGUI()
    {
        if (EditorGUILayout.DropdownButton(new GUIContent(this.selectionPicker.SelectedKey), FocusType.Keyboard))
        {
            this.selectionPicker.WindowDimensions = new Vector2(enumChoiceDropdownButton.width, 250f);
            PopupWindow.Show(enumChoiceDropdownButton, this.selectionPicker);
        }
        if (Event.current.type == EventType.Repaint)
        {
            this.enumChoiceDropdownButton = GUILayoutUtility.GetLastRect();
        }
    }

    public void SetSelectedKey(string key)
    {
        this.selectionPicker.SetSelectedKey(key);
    }

    public string GetSelectedKey()
    {
        return this.selectionPicker.SelectedKey;
    }
}
