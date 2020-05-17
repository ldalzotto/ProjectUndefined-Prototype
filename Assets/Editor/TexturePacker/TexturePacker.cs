using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Editor_TexturePacker
{
    public class TexturePacker : EditorWindow
    {
        [MenuItem("Graphics/TexturePacker")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TexturePacker));
        }

        private Texture2D redTexture;
        private Texture2D greenTexture;
        private Texture2D blueTexture;
        private Texture2D alphaTexture;

        private Texture2D targetTexture;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("(R)", EditorStyles.miniBoldLabel);
            this.redTexture = (Texture2D)EditorGUILayout.ObjectField(this.redTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("(G)", EditorStyles.miniBoldLabel);
            this.greenTexture = (Texture2D)EditorGUILayout.ObjectField(this.greenTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("(B)", EditorStyles.miniBoldLabel);
            this.blueTexture = (Texture2D)EditorGUILayout.ObjectField(this.blueTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("(A)", EditorStyles.miniBoldLabel);
            this.alphaTexture = (Texture2D)EditorGUILayout.ObjectField(this.alphaTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("GENERATE"))
            {
                this.PackRGBA();
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Target", EditorStyles.miniBoldLabel);
            this.targetTexture = (Texture2D)EditorGUILayout.ObjectField(this.targetTexture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            EditorGUILayout.EndVertical();
        }

        private void PackRGBA()
        {
            this.targetTexture = new Texture2D(this.redTexture.width, this.redTexture.height, TextureFormat.RGBA32, false, true);
            for (var i = 0; i < targetTexture.width; i++)
            {
                for (var j = 0; j < targetTexture.height; j++)
                {
                    Color packedColor =
                        new Color(
                            this.redTexture.GetPixel(i, j).r,
                            this.greenTexture.GetPixel(i, j).r,
                            this.blueTexture.GetPixel(i, j).r,
                            this.alphaTexture.GetPixel(i, j).r
                            );
                    targetTexture.SetPixel(i, j, packedColor);
                }
            }
            targetTexture.Apply();
            var redTextureAssetpath = AssetDatabase.GetAssetPath(this.redTexture);
            var splittedPath = redTextureAssetpath.Split('/');
            string targetTexturePath = "";
            for (var i = 1; i < splittedPath.Length - 1; i++)
            {
                targetTexturePath = string.Concat(targetTexturePath, splittedPath[i] + "/");
            }
            targetTexturePath += "TargetTexture.png";

            var pngByte = targetTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + targetTexturePath, pngByte);
        }
    }

}
