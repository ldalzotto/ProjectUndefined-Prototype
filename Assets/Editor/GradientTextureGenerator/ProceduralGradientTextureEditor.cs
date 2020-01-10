using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class ProceduralGradientTextureEditor : EditorWindow
    {
        [MenuItem("Gradient/ProceduralGradientTextureEditor")]
        static void Init()
        {
            var window = GetWindow<ProceduralGradientTextureEditor>("Texture Previewer");
            window.Show();
        }

        [MenuItem("Assets/Gradient/ProceduralGradientTextureEditor")]
        static void InitFromTexture()
        {
            SourceTexture = null;
            ProceduralGradientTextureEditorProfile = null;

            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject is Texture2D selectedTexture)
                {
                    ProceduralGradientTextureEditor.SourceTexture = selectedTexture;
                }
                else if (selectedObject is ProceduralGradientTextureEditorProfile ProceduralGradientTextureEditorProfile)
                {
                    ProceduralGradientTextureEditor.ProceduralGradientTextureEditorProfile = ProceduralGradientTextureEditorProfile;
                }
            }

            Init();
        }

        private static Texture2D SourceTexture;
        private static ProceduralGradientTextureEditorProfile ProceduralGradientTextureEditorProfile;

        private Editor ProceduralGradientTextureEditorProfileEditor;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            SourceTexture = EditorGUILayout.ObjectField(SourceTexture, typeof(Texture2D)) as Texture2D;
            if (EditorGUI.EndChangeCheck())
            {
                ProceduralGradientTextureEditorProfile = null;
                ProceduralGradientTextureEditorProfileEditor = null;
            }

            EditorGUI.BeginChangeCheck();
            ProceduralGradientTextureEditorProfile = EditorGUILayout.ObjectField(ProceduralGradientTextureEditorProfile, typeof(ProceduralGradientTextureEditorProfile)) as ProceduralGradientTextureEditorProfile;
            if (EditorGUI.EndChangeCheck())
            {
                this.ProceduralGradientTextureEditorProfileEditor = null;
            }


            if (ProceduralGradientTextureEditorProfile != null)
            {
                if (this.ProceduralGradientTextureEditorProfileEditor == null)
                {
                    this.ProceduralGradientTextureEditorProfileEditor = Editor.CreateEditor(ProceduralGradientTextureEditorProfile);
                }

                if (this.ProceduralGradientTextureEditorProfileEditor != null)
                {
                    this.ProceduralGradientTextureEditorProfileEditor.OnInspectorGUI();
                }
            }

            if (SourceTexture != null && ProceduralGradientTextureEditorProfile != null)
            {
                if (GUILayout.Button("GO"))
                {
                    var newTexture = new Texture2D(SourceTexture.width, SourceTexture.width, TextureFormat.ARGB32, false, true);
                    newTexture.filterMode = FilterMode.Point;

                    Color[] newColors = new Color[SourceTexture.width * SourceTexture.width];
                    for (var w = 0; w <= SourceTexture.width - 1; w++)
                    {
                        for (var h = 0; h <= SourceTexture.width - 1; h++)
                        {
                            if (ProceduralGradientTextureEditorProfile.AllGradients.Count - 1 >= w)
                            {
                                newColors[(h * SourceTexture.width) + w] = ProceduralGradientTextureEditorProfile.AllGradients[w].Evaluate((1 - ((float) h / (float) SourceTexture.width)));
                            }
                        }
                    }

                    newTexture.SetPixels(newColors);
                    newTexture.Apply();
                    File.WriteAllBytes(AssetDatabase.GetAssetPath(SourceTexture), newTexture.EncodeToPNG());
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(SourceTexture), ImportAssetOptions.ForceUpdate);
                }
            }
        }
    }
}