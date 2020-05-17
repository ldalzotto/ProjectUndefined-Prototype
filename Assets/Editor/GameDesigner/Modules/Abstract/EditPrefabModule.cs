using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Editor_GameDesigner
{
    public abstract class EditPrefabModule<T> : IGameDesignerModule where T : UnityEngine.Object
    {
        protected GameObject currentSelectedObjet;
        public virtual void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.currentSelectedObjet = GameDesignerHelper.GetCurrentSceneSelectedObject();
            EditorGUI.BeginDisabledGroup(this.IsDisabled());
            if (GUILayout.Button("EDIT"))
            {
                this.OnEdit(this.currentSelectedObjet.GetComponent<T>());
                EditorUtility.SetDirty(this.currentSelectedObjet);
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual bool IsDisabled()
        {
            return this.currentSelectedObjet == null || this.currentSelectedObjet.GetComponent<T>() == null;
        }

        protected abstract void OnEdit(T component);

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}