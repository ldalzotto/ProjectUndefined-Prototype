using System;
using System.IO;
using OdinSerializer;
using UnityEditor;
using UnityEngine;

public class CreateScriptableObjectEditor
{
    [MenuItem("Assets/Create/ScriptableObject")]
    private static void CreateScriptableObject()
    {
        ClassSelectionEditorWindow.Show(Vector2.zero, typeof(SerializedScriptableObject), (Type selectedType) => { CreateAsset(selectedType); });

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        void CreateAsset(Type soType)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(soType);

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + soType.ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}