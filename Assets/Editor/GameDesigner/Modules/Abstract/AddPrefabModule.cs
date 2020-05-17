using System;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public abstract class AddPrefabModule<T> : IGameDesignerModule where T : UnityEngine.Object
    {

        protected GameObject currentSelectedObjet;
        protected T prefabToAdd;
        public virtual void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.currentSelectedObjet = GameDesignerHelper.GetCurrentSceneSelectedObject();
            this.prefabToAdd = (T)EditorGUILayout.ObjectField(this.prefabToAdd, typeof(T), false);
            EditorGUI.BeginDisabledGroup(!this.IsAbleToAdd());
            if (GUILayout.Button("ADD TO SCENE"))
            {
                if (this.prefabToAdd != null)
                {
                    var parent = this.ParentGameObject.Invoke();
                    if (parent != null)
                    {
                        PrefabUtility.InstantiatePrefab(this.prefabToAdd, this.ParentGameObject.Invoke().transform);
                    }
                    else
                    {
                        Debug.LogError("Parent not found.");
                    }
                }
                else
                {
                    Debug.LogError("Prefab not set.");
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual bool IsAbleToAdd() { return true; }
        protected abstract Func<GameObject> ParentGameObject { get; }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}