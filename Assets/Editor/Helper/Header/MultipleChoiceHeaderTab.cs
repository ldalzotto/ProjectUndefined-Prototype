#if UNITY_EDITOR

using OdinSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class MultipleChoiceHeaderTab<T> : SerializedScriptableObject
{
    public abstract Dictionary<string, MultipleChoiceHeaderTabSelectionProfile> ConfigurationSelection { get; }

    public abstract Dictionary<string, T> Configurations { get; }

    [SerializeField]
    private Vector2 headerSelectionScrollPosition;

    public void OnSelected(string newSelectedTag)
    {
        this.ConfigurationSelection[newSelectedTag].IsSelected = true;
        this.ConfigurationSelection.Keys.Where(p => p != newSelectedTag).ToList().ForEach((k) => { this.ConfigurationSelection[k].IsSelected = false; });
    }

    public T GetSelectedConf()
    {
        var selectedEntry = this.ConfigurationSelection.Where(v => v.Value.IsSelected);
        if (selectedEntry != null && selectedEntry.Count() > 0)
        {
            return this.Configurations[selectedEntry.First().Key];
        }
        return default(T);

    }

    public static string ComputeSelectionKey(Type enumType)
    {
        return enumType.ToString();
    }

    public void GUITick()
    {

        EditorGUILayout.Space();

        this.headerSelectionScrollPosition = EditorGUILayout.BeginScrollView(this.headerSelectionScrollPosition);
        EditorGUILayout.BeginHorizontal();
        var configurationsSelections = this.ConfigurationSelection;
        foreach (var configurationSelection in configurationsSelections)
        {

            configurationsSelections[configurationSelection.Key].IsSelected =
                OnToggleDisableOthers(configurationSelection.Value.TabName, configurationsSelections[configurationSelection.Key].ButtonStyle, configurationsSelections[configurationSelection.Key].IsSelected,
                 () => { this.OnSelected(configurationSelection.Key); });

        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
    }

    private bool OnToggleDisableOthers(string text, GUIStyle style, bool currentTab, Action OnActivated)
    {
        EditorGUI.BeginChangeCheck();
        var selected = GUILayout.Toggle(currentTab, text, style);
        if (EditorGUI.EndChangeCheck())
        {
            if (selected)
            {
                OnActivated.Invoke();
            }
        }
        return selected;
    }

}

[System.Serializable]
public class MultipleChoiceHeaderTabSelectionProfile
{

    [SerializeField]
    private bool isSelected;
    [SerializeField] private string tabName;
    private GUIStyle buttonStyle;

    public MultipleChoiceHeaderTabSelectionProfile(string tabName)
    {
        this.isSelected = false;
        this.tabName = tabName;
    }

    public bool IsSelected
    {
        get => isSelected; set
        {
            this.isSelected = value;
        }
    }
    public GUIStyle ButtonStyle
    {
        get
        {
            if (this.buttonStyle == null)
            {
                this.buttonStyle = EditorStyles.miniButton;
            }
            return this.buttonStyle;
        }
    }

    public string TabName { get => tabName; }
}

#endif //UNITY_EDITOR