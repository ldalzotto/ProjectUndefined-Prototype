using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class TreePickerPopup : PopupWindowContent
{

    public const string NAME_SEPARATION = "//";

    #region Search Bars
    private SearchField treeSearchField;
    private string searchString = string.Empty;
    private Regex searchRegex;
    #endregion //Search Bars

    #region Keys configuration
    private List<string> sortedKeys;
    private TreePathNode pathRootNode;
    #endregion

    private Action OnSelectionChange;
    private Action repaintAction;
    private string selectedKey;
    private GUIStyle selectedStyle;
    private Color nonSelectableColor = new Color(0.8f, 0.8f, 0.8f);
    private Vector2 windowDimensions;

    private Vector2 scrollPosition;
    private int currentIndentLevel;

    public override Vector2 GetWindowSize()
    {
        return this.windowDimensions;
    }

    public TreePickerPopup(List<string> sortedKeys, Action OnSelectionChange, string oldSelectedKey)
    {
        this.selectedKey = oldSelectedKey;
        this.sortedKeys = sortedKeys;
        this.OnSelectionChange = OnSelectionChange;
    }

    public string SelectedKey { get => selectedKey; }
    public Action RepaintAction { set => repaintAction = value; }
    public Vector2 WindowDimensions { set => windowDimensions = value; }

    public void SetSelectedKey(string newKey)
    {
        this.selectedKey = newKey;
        this.OnSelectionChange();
    }

    public override void OnGUI(Rect rect)
    {
        this.Init();

        this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical();

        EditorGUI.BeginChangeCheck();
        this.searchString = this.treeSearchField.OnGUI(this.searchString);
        if (EditorGUI.EndChangeCheck())
        {
            this.searchRegex = new Regex(this.searchString, RegexOptions.IgnoreCase);
        }
        EditorGUILayout.Space();

        this.GUITreeDictionary(this.pathRootNode.ChildNodes);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck())
        {
            if (this.repaintAction != null)
            {
                this.repaintAction.Invoke();
            }
        }
    }

    private void GUITreeDictionary(Dictionary<string, TreePathNode> nodes)
    {
        foreach (var k in nodes)
        {
            //filtering
            if (this.searchRegex.Match(k.Value.Key).Success)
            {

                if (k.Value.ChildNodes.Count > 0)
                {
                    var oldBackgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = this.nonSelectableColor;
                    GUILayout.Button(k.Key.ToString().Insert(0, this.SpaceFromIndentLevel()), this.selectedStyle);
                    GUI.backgroundColor = oldBackgroundColor;
                    this.currentIndentLevel += 1;
                    this.GUITreeDictionary(k.Value.ChildNodes);
                    this.currentIndentLevel -= 1;
                }
                else
                {
                    var oldBackgroundColor = GUI.backgroundColor;
                    if (k.Value.Key == this.selectedKey)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(k.Key.ToString().Insert(0, this.SpaceFromIndentLevel()), this.selectedStyle))
                    {
                        this.selectedKey = k.Value.Key;
                        if (this.OnSelectionChange != null)
                        {
                            this.OnSelectionChange.Invoke();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = oldBackgroundColor;
                }
            }
            else
            {
                if (k.Value.ChildNodes.Count > 0)
                {
                    var oldBackgroundColor = GUI.backgroundColor;
                    this.currentIndentLevel += 1;
                    this.GUITreeDictionary(k.Value.ChildNodes);
                    this.currentIndentLevel -= 1;
                }
            }
        }
    }

    private void Init()
    {
        if (this.selectedStyle == null)
        {
            this.selectedStyle = new GUIStyle(EditorStyles.label);
            this.selectedStyle.normal.background = Texture2D.whiteTexture;
        }
        if (this.treeSearchField == null)
        {
            this.treeSearchField = new SearchField();
            this.treeSearchField.SetFocus();
        }
        if (this.searchRegex == null)
        {
            this.searchRegex = new Regex(this.searchString);
        }
        if (this.pathRootNode == null)
        {
            this.BuildPathNodes();
        }
        this.currentIndentLevel = 0;
    }

    private void BuildPathNodes()
    {
        this.pathRootNode = new TreePathNode(new Dictionary<string, TreePathNode>(), string.Empty);
        foreach (var key in this.sortedKeys)
        {
            var paths = this.ExtractKeyPath(key);
            paths.Reverse();
            var pathStack = new Stack<string>(paths);
            this.pathRootNode.InsertPath(ref pathStack, key);
        }
    }

    private List<string> ExtractKeyPath(string key)
    {
        return key.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private string SpaceFromIndentLevel()
    {
        var str = string.Empty;
        for (var i = 0; i < this.currentIndentLevel; i++)
        {
            str += "  ";
        }
        return str;
    }

}

public class TreePathNode
{
    public Dictionary<string, TreePathNode> ChildNodes;
    public string Key;

    public TreePathNode(Dictionary<string, TreePathNode> childNodes, string key)
    {
        ChildNodes = childNodes;
        Key = key;
    }

    public void InsertPath(ref Stack<string> pathStack, string key)
    {
        if (pathStack != null && pathStack.Count > 0)
        {
            var currentPath = pathStack.Pop();
            if (!this.ChildNodes.ContainsKey(currentPath))
            {
                this.ChildNodes.Add(currentPath, new TreePathNode(new Dictionary<string, TreePathNode>(), key));
                this.ChildNodes[currentPath].InsertPath(ref pathStack, key);
            }
            else
            {
                this.ChildNodes[currentPath].InsertPath(ref pathStack, key);
            }
        }
    }

}