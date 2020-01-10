using System;
using InteractiveObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractiveObjectInitializer))]
public class InteractiveObjectInitializerCustomEdtor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Show GIZMO"))
        {
            var InteractiveObjectExplorer = EditorWindow.GetWindow<InteractiveObjectExplorer>();
            if (InteractiveObjectExplorer != null)
            {
                Debug.Log(this.target.GetType().Name);
                InteractiveObjectExplorer.SelectGizmoFromInteractiveObjectComponent(this.target as InteractiveObjectInitializer);
            }
        }

        base.OnInspectorGUI();
    }
}