using System;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public abstract class SetModelModule<T> : IGameDesignerModule where T : UnityEngine.Object
    {
        protected GameObject ModelObject;

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {

            var currentSelectedObj = GameDesignerHelper.GetCurrentSceneSelectedObject();

            this.ModelObject = (GameObject)EditorGUILayout.ObjectField("New Model : ", this.ModelObject, typeof(GameObject), false);

            EditorGUI.BeginDisabledGroup(this.ModelObject == null || currentSelectedObj == null || currentSelectedObj.GetComponent<T>() == null);
            if (GUILayout.Button("SET MODEL"))
            {
                this.OnClick(currentSelectedObj);
            }
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void OnClick(GameObject currentSelectedObj)
        {
            var parent = this.FindParent.Invoke(currentSelectedObj.GetComponent<T>());
            PrefabUtility.InstantiatePrefab(this.ModelObject, parent);
            EditorUtility.SetDirty(parent);
        }

        protected abstract Func<T, Transform> FindParent { get; }


        public void OnDisabled()
        {

        }

        public virtual void OnEnabled()
        {

        }
    }
}