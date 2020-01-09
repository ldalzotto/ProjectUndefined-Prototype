using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class ProceduralGradientTextureEditor : EditorWindow
    {
        [MenuItem("Test/ProceduralGradientTextureEditor")]
        static void Init()
        {
            var window = GetWindow<ProceduralGradientTextureEditor>("Texture Previewer");
            window.Show();
        }

        private Texture2D SourceTexture;
        public ProceduralGradientTextureEditorProfile ProceduralGradientTextureEditorProfile;

        private void OnGUI()
        {
            this.SourceTexture = EditorGUILayout.ObjectField(this.SourceTexture, typeof(Texture2D)) as Texture2D;
            this.ProceduralGradientTextureEditorProfile = EditorGUILayout.ObjectField(this.ProceduralGradientTextureEditorProfile, typeof(ProceduralGradientTextureEditorProfile)) as ProceduralGradientTextureEditorProfile;
            if (this.SourceTexture != null && this.ProceduralGradientTextureEditorProfile != null)
            {
                if (GUILayout.Button("GO"))
                {
                    var newTexture = new Texture2D(this.SourceTexture.width, this.SourceTexture.width, TextureFormat.ARGB32, false, true);
                    newTexture.filterMode = FilterMode.Point;

                    Color[] newColors = new Color[this.SourceTexture.width * this.SourceTexture.width];
                    for (var w = 0; w <= this.SourceTexture.width - 1; w++)
                    {
                        for (var h = 0; h <= this.SourceTexture.width - 1; h++)
                        {
                            if (this.ProceduralGradientTextureEditorProfile.AllGradients.Count - 1 >= w)
                            {
                                newColors[(h * this.SourceTexture.width) + w] = this.ProceduralGradientTextureEditorProfile.AllGradients[w].Evaluate((1 - ((float) h / (float) this.SourceTexture.width)));
                            }
                        }
                    }

                    newTexture.SetPixels(newColors);
                    newTexture.Apply();
                    File.WriteAllBytes(AssetDatabase.GetAssetPath(this.SourceTexture), newTexture.EncodeToPNG());
                }
            }
        }
    }
}