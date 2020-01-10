using UnityEditor;
using UnityEngine;


public class ToonShaderConverter
{
    [MenuItem("Assets/Material/Convert to toon")]
    public static void ConvertToToon()
    {
        foreach (var selectedObject in Selection.objects)
        {
            if (selectedObject is Material selectedMaterial)
            {
                selectedMaterial.shader = Shader.Find("Unlit/ToonShader");
                selectedMaterial.SetColor("_BaseColor", Color.white);
                selectedMaterial.SetTexture("_DiffuseRamp", AssetFinder.SafeSingleAssetFind<Texture2D>("ToonRamp"));
            }
        }
    }
}