using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MaterialRandomizerEditor : EditorWindow
{
    // Add menu named "My Window" to the Window menu
    [MenuItem("Test/MaterialRandomizerEditor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MaterialRandomizerEditor window = (MaterialRandomizerEditor) EditorWindow.GetWindow(typeof(MaterialRandomizerEditor));
        window.Show();
    }

    private MaterialRandomizerEditorInternalProfile MaterialRandomizerEditorInternalProfile;
    private Editor MaterialRandomizerEditorInternalProfileEditor;

    private void OnEnable()
    {
        this.MaterialRandomizerEditorInternalProfile = new MaterialRandomizerEditorInternalProfile();
        this.MaterialRandomizerEditorInternalProfileEditor = Editor.CreateEditor(this.MaterialRandomizerEditorInternalProfile);
    }

    private void OnGUI()
    {
        this.MaterialRandomizerEditorInternalProfileEditor.OnInspectorGUI();
        if (GUILayout.Button("RANDOMIZE"))
        {
            foreach (var renderer in this.MaterialRandomizerEditorInternalProfile.RandomizedRenderers)
            {
                renderer.sharedMaterial = this.MaterialRandomizerEditorInternalProfile.MaterialPool[Random.Range(0, this.MaterialRandomizerEditorInternalProfile.MaterialPool.Count)];
            }
        }
    }
}

[Serializable]
public class MaterialRandomizerEditorInternalProfile : SerializedScriptableObject
{
    public List<Renderer> RandomizedRenderers;
    public List<Material> MaterialPool;
}