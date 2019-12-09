using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DynamicEditorCreation
{
    private static DynamicEditorCreation Instance;

    public static DynamicEditorCreation Get()
    {
        if (Instance == null)
        {
            Instance = new DynamicEditorCreation();
        }

        return Instance;
    }

    public DynamicEditorCreation()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            ClearCreatedEditors();
        }
    }

    public List<Editor> CreatedEditors = new List<Editor>();

    [MenuItem("EditorTool/DynamicEditorClear")]
    public static void ClearCreatedEditors()
    {
        for (var i = Get().CreatedEditors.Count - 1; i >= 0; i--)
        {
            var createdEditor = Get().CreatedEditors[i];
            if (createdEditor != null)
            {
                Editor.DestroyImmediate(createdEditor);
            }
        }

        Get().CreatedEditors.Clear();
    }

    public Editor CreateEditor(Object obj)
    {
        var editor = Editor.CreateEditor(obj);
        this.CreatedEditors.Add(editor);
        return editor;
    }
}