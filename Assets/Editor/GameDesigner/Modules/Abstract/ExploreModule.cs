using UnityEngine;
using System.Collections;
using Editor_MainGameCreationWizard;
using System.Collections.Generic;
using UnityEditor;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public abstract class ExploreModule : IGameDesignerModule
    {

        protected CommonGameConfigurations commonGameConfigurations;
        public abstract void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile);

        public void OnDisabled()
        {
       
        }

        public virtual void OnEnabled()
        {
            this.commonGameConfigurations = new CommonGameConfigurations();
            EditorInformationsHelper.InitProperties(ref this.commonGameConfigurations);

        }

        protected void DisplayObjects<T>(string label, List<T> objs, ref Dictionary<T, Editor> cachedEditors) where T : UnityEngine.Object
        {
            if (objs != null && objs.Count > 0)
            {
                EditorGUILayout.LabelField(label);
                EditorGUI.indentLevel += 1;
                foreach (var obj in objs)
                {
                    EditorGUILayout.ObjectField(obj, typeof(T), false);
                    if (obj != null)
                    {
                        if (!cachedEditors.ContainsKey(obj))
                        {
                            cachedEditors[obj] = DynamicEditorCreation.Get().CreateEditor(obj);
                        }
                        cachedEditors[obj].OnInspectorGUI();
                        EditorGUILayout.Separator();
                    }
                }
                EditorGUI.indentLevel -= 1;
            }
        }
    }
}
