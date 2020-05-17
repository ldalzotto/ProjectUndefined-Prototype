using UnityEditor;
using UnityEngine;

public class WaveNoiseV2ShaderGUI : ShaderGUI
{

    public MaterialKeywordResolve VertexDisplacementKeyword = new MaterialKeywordResolve("VERTEX_DISPLACEMENT");

    public MaterialProperty Color;
    public MaterialProperty Smoothness;
    public MaterialProperty Metallic;

    public MaterialKeywordResolve NormalMapKeyWord = new MaterialKeywordResolve("NORMAL_MAP");
    public MaterialProperty IsNormalMap;
    public MaterialProperty NormalMap;

    public MaterialKeywordResolve Emission = new MaterialKeywordResolve("EMISISON");
    public MaterialProperty IsEmisison;
    public MaterialProperty EmissionColor;

    public MaterialProperty IsVertexDisplaced;

    public MaterialProperty MaxIntensity;
    public MaterialProperty MinIntensity;

    public MaterialProperty VertexDisplacementType;

    public MaterialKeywordResolve DirectionTextureKeyWord = new MaterialKeywordResolve("DIRECTION_TEXTURE");
    public MaterialProperty HasDirectionTexture;
    public MaterialProperty DirectionTexture;

    public MaterialKeywordResolve IsNoise = new MaterialKeywordResolve("_VERTEXDISPLACEMENTTYPE_NOISE");
    public MaterialProperty DisplacementFactorMap;
    public MaterialProperty WorldSpaceDirection;
    public MaterialProperty NoiseSpeed;
    public MaterialProperty NoiseFrequency;

    public MaterialKeywordResolve IsWave = new MaterialKeywordResolve("_VERTEXDISPLACEMENTTYPE_WAVE");
    public MaterialProperty WaveMap;
    public MaterialProperty MaxSpeed;
    public MaterialProperty MaxFrequency;

    private void DoInit(MaterialEditor materialEditor, MaterialProperty[] properties, Material material)
    {
        VertexDisplacementKeyword.Resolve(material);
        NormalMapKeyWord.Resolve(material);
        IsNoise.Resolve(material);
        IsWave.Resolve(material);
        DirectionTextureKeyWord.Resolve(material);
        Emission.Resolve(material);

        this.IsVertexDisplaced = FindProperty("_IsVertexDisplaced", properties);
        this.Color = FindProperty("_Color", properties);
        this.Smoothness = FindProperty("_Glossiness", properties);
        this.Metallic = FindProperty("_Metallic", properties);

        this.IsEmisison = FindProperty("_IsEmission", properties);
        this.EmissionColor = FindProperty("_EmissionColor", properties);

        this.IsNormalMap = FindProperty("_IsNormalMap", properties);
        this.NormalMap = FindProperty("_BumpMap", properties);

        this.MaxIntensity = FindProperty("_MaxIntensity", properties);
        this.MinIntensity = FindProperty("_MinIntensity", properties);

        this.VertexDisplacementType = FindProperty("_VertexDisplacementType", properties);

        this.HasDirectionTexture = FindProperty("_IsDirectionTexture", properties);
        this.DirectionTexture = FindProperty("_DirectionTexture", properties);

        this.DisplacementFactorMap = FindProperty("_DisplacementFactorMap", properties);
        this.WorldSpaceDirection = FindProperty("_WorldSpaceDirection", properties);
        this.NoiseSpeed = FindProperty("_NoiseSpeed", properties);
        this.NoiseFrequency = FindProperty("_NoiseFrequency", properties);

        this.WaveMap = FindProperty("_WaveMap", properties);
        this.MaxSpeed = FindProperty("_MaxSpeed", properties);
        this.MaxFrequency = FindProperty("_MaxFrequency", properties);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var material = (Material)materialEditor.target;
        this.DoInit(materialEditor, properties, material);

        materialEditor.ShaderProperty(this.Color, "Base color");
        materialEditor.ShaderProperty(this.Smoothness, "Smoothness");
        materialEditor.ShaderProperty(this.Metallic, "Metallic");

        materialEditor.ShaderProperty(this.IsEmisison, "has Emission?");
        if (this.Emission.KeywordValue)
        {
            materialEditor.ShaderProperty(this.EmissionColor, "Emission color");
        }

        materialEditor.ShaderProperty(this.IsNormalMap, "Normal map ?");
        if (this.NormalMapKeyWord.KeywordValue)
        {
            EditorGUI.indentLevel += 1;
            materialEditor.TexturePropertySingleLine(new GUIContent("Normal map"), this.NormalMap);
            materialEditor.TextureScaleOffsetProperty(this.NormalMap);
            EditorGUI.indentLevel -= 1;
        }

        materialEditor.ShaderProperty(this.IsVertexDisplaced, "Is Vertex Displaced ?");

        if (this.VertexDisplacementKeyword.KeywordValue)
        {
            EditorGUI.indentLevel += 1;
            materialEditor.ShaderProperty(this.MaxIntensity, "Max Intensity");
            materialEditor.ShaderProperty(this.MinIntensity, "Min Intensity");
            materialEditor.ShaderProperty(this.VertexDisplacementType, "Vertex displacement type");

            materialEditor.ShaderProperty(this.HasDirectionTexture, "Direction texture ?");
            if (DirectionTextureKeyWord.KeywordValue)
            {
                materialEditor.TexturePropertySingleLine(new GUIContent("Direction texture"), this.DirectionTexture);
            }

            if (IsNoise.KeywordValue)
            {
                EditorGUI.indentLevel += 1;
                materialEditor.TexturePropertySingleLine(new GUIContent("Displacement factor map"), this.DisplacementFactorMap);
                materialEditor.ShaderProperty(this.WorldSpaceDirection, "World space direction");
                materialEditor.ShaderProperty(this.NoiseSpeed, "Noise speed");
                materialEditor.ShaderProperty(this.NoiseFrequency, "Noise frequency");
                EditorGUI.indentLevel -= 1;
            }
            else if (IsWave.KeywordValue)
            {
                EditorGUI.indentLevel += 1;
                materialEditor.TexturePropertySingleLine(new GUIContent("Wave map"), this.WaveMap);
                materialEditor.ShaderProperty(this.MaxSpeed, "Max speed");
                materialEditor.ShaderProperty(this.MaxFrequency, "Max frequency");
                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
        }
        materialEditor.RenderQueueField();

    }
}

public class MaterialKeywordResolve
{
    private string KeywordName;
    public bool KeywordValue;

    public MaterialKeywordResolve(string keywordName)
    {
        KeywordName = keywordName;
    }

    public void Resolve(Material material)
    {
        this.KeywordValue = material.IsKeywordEnabled(this.KeywordName);
    }


    public void Set(bool value, Material material)
    {
        this.KeywordValue = value;
        if (value)
        {
            material.EnableKeyword(this.KeywordName);
        }
        else
        {
            material.DisableKeyword(this.KeywordName);
        }
    }
}