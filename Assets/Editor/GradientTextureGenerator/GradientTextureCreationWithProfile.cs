using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class GradientTextureCreationWithProfile
    {
        [MenuItem("Assets/Gradient/CreateTexture")]
        public static void CreateGradientTexture()
        {
            var window = EditorWindow.GetWindow<GradientTextureCreationInputWindow>("GradientTextureCreationInputWindow");
            window.Init((name, size) =>
            {
                var texture = new Texture2D(size, size, TextureFormat.ARGB32, false, true);
                texture.SetPixels(new Color[size * size]);
                texture.Apply();
                var path = GetCurrentSelectedFolder() + "/" + name + ".png";
                File.WriteAllBytes(path, texture.EncodeToPNG());
                AssetDatabase.ImportAsset(path);

                var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                texture2D.filterMode = FilterMode.Point;
                texture2D.wrapMode = TextureWrapMode.Clamp;
                EditorUtility.SetDirty(texture2D);
                AssetDatabase.SaveAssets();

                var importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture2D)) as TextureImporter;
                importer.filterMode = FilterMode.Point;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;

                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture2D), ImportAssetOptions.ForceUpdate);

                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ProceduralGradientTextureEditorProfile>(), GetCurrentSelectedFolder() + "/" + name + "_ProceduralGradientTextureEditorProfile.asset");
            });
            window.Show();
        }

        public static string GetCurrentSelectedFolder()
        {
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                break;
            }

            return path;
        }
    }

    class GradientTextureCreationInputWindow : EditorWindow
    {
        /*
        public static void Init()
        {
            var window = GetWindow<GradientTextureCreationInputWindow>("GradientTextureCreationInputWindow");
            window.Show();
        }
        */

        private Action<string, int> OnOkClickedAction;

        public void Init(Action<string, int> OnOKClicked)
        {
            this.OnOkClickedAction = OnOKClicked;
        }

        private string Name;
        private int Size;

        private void OnGUI()
        {
            this.Name = EditorGUILayout.TextField(new GUIContent("Name : "), this.Name);
            this.Size = EditorGUILayout.IntField(new GUIContent("Size : "), this.Size);

            if (GUILayout.Button("OK"))
            {
                this.OnOkClickedAction.Invoke(this.Name, this.Size);
                this.Close();
            }
        }
    }
}