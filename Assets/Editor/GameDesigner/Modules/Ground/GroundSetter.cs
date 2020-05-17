using System;
using RangeObjects;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor_GameDesigner
{
    [Serializable]
    public class GroundEffectAdd : IGameDesignerModule
    {
        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            var obj = Selection.activeObject as GameObject;
            if (obj != null && obj.scene == null) obj = null;

            EditorGUILayout.ObjectField(obj, typeof(Object), false);
            if (GUILayout.Button("SET GROUND EFFECT"))
                if (obj != null)
                {
                    if (obj.GetComponent<GroundEffectType>() == null) obj.AddComponent<GroundEffectType>();

                    obj.layer = LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER);
                }
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
        }
    }
}