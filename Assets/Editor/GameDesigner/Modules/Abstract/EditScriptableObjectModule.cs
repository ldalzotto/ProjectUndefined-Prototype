using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Editor_GameDesigner
{
    public abstract class EditScriptableObjectModule<C> : IGameDesignerModule where C : UnityEngine.Object
    {
        private Editor cachedEditor;
        private C lastFrameObj;

        protected abstract Func<C, ScriptableObject> scriptableObjectResolver { get; }

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.OnEnabled();
            var rawSelectedObj = GameDesignerHelper.GetCurrentSceneSelectedObject();
            C currentSelectedObj = default(C);

            if (rawSelectedObj != null)
            {
                currentSelectedObj = rawSelectedObj.GetComponent<C>();
            }

            if (currentSelectedObj == null)
            {
                this.cachedEditor = null;
            }
            if (currentSelectedObj != null)
            {
                if (currentSelectedObj != this.lastFrameObj)
                {
                    CreateEditor(currentSelectedObj);
                }

                if (GUILayout.Button("REFRESH", GUILayout.Width(50f)))
                {
                    CreateEditor(currentSelectedObj);
                }

                if (this.cachedEditor != null)
                {
                    this.cachedEditor.OnInspectorGUI();
                }
            }
            this.lastFrameObj = currentSelectedObj;
        }

        private void CreateEditor(C npcAIManager)
        {
            var so = scriptableObjectResolver.Invoke(npcAIManager);
            if (so != null)
            {
                this.cachedEditor = DynamicEditorCreation.Get().CreateEditor(so);
            }
        }

        public virtual void OnDisabled() { }
        public virtual void OnEnabled() { }
    }
}

