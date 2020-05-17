using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Editor_GameDesigner
{
    public class GameDesignerHelper 
    {
        public static GameObject GetCurrentSceneSelectedObject()
        {
            var currentSelectedObj = Selection.activeObject as GameObject;
            if (currentSelectedObj != null && currentSelectedObj.scene == null)
            {
                currentSelectedObj = null;
            }
            EditorGUILayout.ObjectField("Current selection : " , currentSelectedObj, typeof(Object), false);
            return currentSelectedObj;
        }
    }
}