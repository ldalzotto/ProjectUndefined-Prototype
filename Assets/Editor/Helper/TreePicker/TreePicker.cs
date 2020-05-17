using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public abstract class TreePicker<T> : AbstractTreePickerGUI<T>
{
    public void GUITick(Action repaintAction)
    {
        this.Init(repaintAction);
        this.TreePickerPopup.OnGUI(new Rect());
    }
}
