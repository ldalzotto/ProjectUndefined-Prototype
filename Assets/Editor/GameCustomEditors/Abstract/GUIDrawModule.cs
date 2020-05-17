using CoreGame;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    public abstract class GUIDrawModule<T, C>
    {
        private bool enabled = true;

        public bool Enabled { get => enabled; }

        public void SetEnabled(bool value)
        {
            this.enabled = value;
        }

        public virtual void EditorGUI(C context, T target)
        {
            this.enabled = GUILayout.Toggle(enabled, this.GetType().Name, EditorStyles.miniButton);
        }

        public abstract void SceneGUI(C context, T target);
    }

    public abstract class IDPickGUIModule<T, C, K, V> : GUIDrawModule<T, C> where K : System.Enum
    {
        public abstract Func<C, ByEnumProperty<K, V>> GetByEnumProperty { get; }

        private int selectedEnumIndex;
        private K selectedKey;

        public override void EditorGUI(C context, T target)
        {
            EditorGUILayout.BeginHorizontal();
            var byEnumProperty = GetByEnumProperty.Invoke(context);
            if (byEnumProperty != null)
            {
                var availableEnums = byEnumProperty.Values.Keys.ToList();
                base.EditorGUI(context, target);
                this.selectedEnumIndex = EditorGUILayout.Popup(this.selectedEnumIndex, availableEnums.ConvertAll(e => e.ToString()).ToArray(), GUILayout.Width(30f));
                if (availableEnums.Count > 0)
                {
                    this.selectedKey = availableEnums[this.selectedEnumIndex];
                }
            }
          
           
            EditorGUILayout.EndHorizontal();
        }

        public override void SceneGUI(C context, T target)
        {
            this.SceneGUI(context, target, this.selectedKey);
        }

        public abstract void SceneGUI(C context, T target, K key);
    }

}