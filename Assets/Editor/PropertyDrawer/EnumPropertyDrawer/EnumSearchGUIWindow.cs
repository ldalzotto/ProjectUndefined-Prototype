using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[System.Serializable]
public class EnumSearchGUIWindow : EditorWindow
{

    private Enum enumType;
    private SearchField searchField;
    private GUIStyle searchFieldStyle;
    private string searchStr;
    private Regex searchRegex;
    private Vector3 enumSearchScrollView;
    private Action<Enum> onEnumSelected;
    private Enum newEnum;

    public Enum NewEnum { get => newEnum; }

    public void Init(Enum enumType, Action<Enum> onEnumSelected)
    {
        this.enumType = enumType;
        this.onEnumSelected = onEnumSelected;
        this.newEnum = enumType;
    }

    private void OnGUI()
    {
        if (this.enumType != null)
        {

            if (this.searchField == null)
            {
                this.searchField = new SearchField();
                this.searchField.SetFocus();
            }
            if (this.searchFieldStyle == null)
            {
                this.searchFieldStyle = new GUIStyle();
                this.searchFieldStyle.padding = EditorStyles.miniButton.padding;
            }

            EditorGUILayout.BeginVertical(this.searchFieldStyle);

            // this.searchField.SetFocus();
            EditorGUI.BeginChangeCheck();
            this.searchStr = this.searchField.OnGUI(this.searchStr);
            if (EditorGUI.EndChangeCheck())
            {
                this.searchRegex = new Regex(this.searchStr);
            }

            EditorGUILayout.EndVertical();

            this.enumSearchScrollView = EditorGUILayout.BeginScrollView(this.enumSearchScrollView, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);

            foreach (var enumName in Enum.GetNames(enumType.GetType()))
            {
                bool display = false;
                if (!string.IsNullOrEmpty(this.searchStr))
                {
                    var m = this.searchRegex.Match(enumName);
                    if (m != null && m.Success)
                    {
                        display = true;
                    }
                }
                else
                {
                    display = true;
                }

                if (display)
                {
                    EditorGUILayout.LabelField(enumName, EditorStyles.miniLabel);
                    var currentFieldArea = GUILayoutUtility.GetLastRect();
                    if (currentFieldArea.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                    {
                        if (this.onEnumSelected != null)
                        {
                            this.newEnum = (Enum)Enum.Parse(this.enumType.GetType(), enumName);
                            this.onEnumSelected.Invoke(this.newEnum);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();

        }
    }
}
