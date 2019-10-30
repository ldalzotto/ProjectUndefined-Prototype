using Timelines;
using UnityEditor;
using UnityEngine;

namespace Timelines_Editor
{
    [CustomEditor(typeof(TimelineInitializerScriptableObject), editorForChildClasses: true)]
    public class TimelineInitializerScriptableObjectCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("GENERATE"))
            {
                (target as TimelineInitializerScriptableObject).ReGenerate();
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}