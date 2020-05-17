using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[System.Serializable]
public class RegexTextFinder
{
    private SearchField searchField;

    [SerializeField]
    private Regex regex;

    [SerializeField]
    private string searchText;

    public RegexTextFinder()
    {
        this.searchField = new SearchField();
        this.searchField.SetFocus();
    }

    public void GUITick()
    {
        if (this.searchField != null)
        {
            EditorGUI.BeginChangeCheck();
            this.searchText = this.searchField.OnGUI(this.searchText);
            if (EditorGUI.EndChangeCheck())
            {
                if (!string.IsNullOrEmpty(this.searchText))
                {
                    this.regex = new Regex(this.searchText, RegexOptions.IgnoreCase);
                }
            }
        }
    }

    public void SetSearchTest(string searchText)
    {
        this.searchText = searchText;
        if (!string.IsNullOrEmpty(this.searchText))
        {
            this.regex = new Regex(this.searchText, RegexOptions.IgnoreCase);
        }
    }

    public bool IsMatchingWith(string comparisonString)
    {
        return (string.IsNullOrEmpty(this.searchText) || (this.regex != null && this.regex.Match(comparisonString).Success));
    }
}
