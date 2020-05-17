using ConfigurationEditor;
using Editor_GameDesigner;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    public abstract class AbstractGameCustomEditor<T, C> : Editor where T : UnityEngine.Object
    {
        protected List<GUIDrawModule<T, C>> drawModules;
        protected C context;

        protected void OnSceneGUI()
        {
            var targetZone = (T)target;
            if (targetZone != null)
            {
                var oldColor = Handles.color;
                var oldGizmoColor = Gizmos.color;
                var oldGUiBackground = GUI.backgroundColor;

                if (this.drawModules != null)
                {
                    Handles.BeginGUI();
                    GUI.backgroundColor = new Color(oldGUiBackground.r, oldGUiBackground.g, oldGUiBackground.b, 0.5f);
                    GUILayout.BeginArea(new Rect(10, 10, 200, 600));

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("*", EditorStyles.miniButtonLeft, GUILayout.Width(25f)))
                    {
                        this.SetAllModules(true);
                    }
                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25f)))
                    {
                        this.SetAllModules(false);
                    }
                    GUILayout.EndHorizontal();

                    foreach (var module in this.drawModules)
                    {
                        module.EditorGUI(this.context, targetZone);
                    }
                    GUILayout.EndArea();
                    Handles.EndGUI();

                    foreach (var module in this.drawModules)
                    {
                        if (module.Enabled)
                        {
                            module.SceneGUI(this.context, targetZone);
                        }
                    }
                }

                Gizmos.color = oldGizmoColor;
                GUI.backgroundColor = oldGUiBackground;
                Handles.color = oldColor;
            }
        }

        private void SetAllModules(bool value)
        {
            foreach (var module in this.drawModules)
            {
                module.SetEnabled(value);
            }
        }

    }

    public abstract class AbstractGameCustomEditorWithLiveSelection<T, C, CONF_TYPE, DESIGN_TYPE> : AbstractGameCustomEditor<T, C> where T : UnityEngine.Object
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN CONFIGURATION"))
            {
                ConfigurationInspector.OpenConfigurationEditor(typeof(CONF_TYPE));
            }
            if (GUILayout.Button("EDIT IN DESIGNER"))
            {
                GameDesignerEditor.InitWithSelectedKey(typeof(DESIGN_TYPE));
            }
            base.OnInspectorGUI();
        }
    }

    public abstract class AbstractConfigurationDataCustomEditor<CONF_TYPE> : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN CONFIGURATION"))
            {
                ConfigurationInspector.OpenConfigurationEditor(typeof(CONF_TYPE));
            }
            base.OnInspectorGUI();
        }
    }
}