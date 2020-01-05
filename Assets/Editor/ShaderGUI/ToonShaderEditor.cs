using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ToonShaderEditor : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        this.SetupFoldoutHeader(materialEditor);
        this.RetrieveMaterialAttributes(properties);
        this.DisplayEditor(materialEditor);
        this.UpdateMaterialKeywords(materialEditor);
    }

    #region Surface Options

    private EditorPersistantBoolVariable surfaceOptionFoldout;
    private MaterialProperty surfaceTypeProp;
    private MaterialProperty blendModeProp;
    private MaterialProperty cullingProp;
    private MaterialProperty alphaClipProp;
    private MaterialProperty alphaCutoffProp;
    private MaterialProperty receiveShadowsProp;

    #endregion

    #region Toon diffuse Options

    private EditorPersistantBoolVariable toonDiffuseOptionsFoldout;
    private MaterialProperty BaseColor;
    private MaterialProperty BaseMap;
    private MaterialProperty DiffuseRamp;

    #endregion

    #region Toon rim options

    private EditorPersistantBoolVariable toonRimOptionsFoldout;
    private MaterialProperty RimMap;
    private MaterialProperty RimPower;
    private MaterialProperty RimOffset;
    private MaterialProperty RimColor;

    #endregion

    #region Toon specular options

    private EditorPersistantBoolVariable toonSpecularOptionsFoldout;
    private MaterialProperty SpecularRamp;
    private MaterialProperty SpecularMap;
    private MaterialProperty SpecularPower;
    private MaterialProperty SpecularColor;

    #endregion

    #region Toon normal options

    private EditorPersistantBoolVariable toonNormalOptionsFoldout;
    private MaterialProperty BumpMap;
    private MaterialProperty BumpScale;

    #endregion

    #region Toon emission options

    private EditorPersistantBoolVariable toonEmissionOptionsFodlout;
    private MaterialProperty EmissionMap;
    private MaterialProperty EmissionColor;

    #endregion

    #region Advanced Options

    private EditorPersistantBoolVariable advancedOptionsFoldout;
    private MaterialProperty queueOffsetProp;
    private const int queueOffsetRange = 50;

    #endregion

    private void SetupFoldoutHeader(MaterialEditor materialEditor)
    {
        Material material = materialEditor.target as Material;
        EditorPersistantBoolVariable.Initialize(ref this.surfaceOptionFoldout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.surfaceOptionFoldout)));
        EditorPersistantBoolVariable.Initialize(ref this.toonDiffuseOptionsFoldout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.toonDiffuseOptionsFoldout)));
        EditorPersistantBoolVariable.Initialize(ref this.toonRimOptionsFoldout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.toonRimOptionsFoldout)));
        EditorPersistantBoolVariable.Initialize(ref this.toonSpecularOptionsFoldout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.toonSpecularOptionsFoldout)));
        EditorPersistantBoolVariable.Initialize(ref this.toonNormalOptionsFoldout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.toonNormalOptionsFoldout)));
        EditorPersistantBoolVariable.Initialize(ref this.toonEmissionOptionsFodlout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.toonEmissionOptionsFodlout)));
        EditorPersistantBoolVariable.Initialize(ref this.advancedOptionsFoldout, EditorPersistantBoolVariable.BuildKeyFromObject(material, nameof(this.advancedOptionsFoldout)));
    }

    private void RetrieveMaterialAttributes(MaterialProperty[] properties)
    {
        this.surfaceTypeProp = FindProperty("_Surface", properties);
        this.blendModeProp = FindProperty("_Blend", properties);
        this.cullingProp = FindProperty("_Cull", properties);
        this.alphaClipProp = FindProperty("_AlphaClip", properties);
        this.alphaCutoffProp = FindProperty("_Cutoff", properties);
        this.receiveShadowsProp = FindProperty("_ReceiveShadows", properties);

        this.BaseColor = FindProperty("_BaseColor", properties);
        this.BaseMap = FindProperty("_BaseMap", properties);
        this.DiffuseRamp = FindProperty("_DiffuseRamp", properties);

        this.RimPower = FindProperty("_RimPower", properties);
        this.RimMap = FindProperty("_RimMap", properties);
        this.RimOffset = FindProperty("_RimOffset", properties);
        this.RimColor = FindProperty("_RimColor", properties);

        this.SpecularRamp = FindProperty("_SpecularRamp", properties);
        this.SpecularMap = FindProperty("_SpecularMap", properties);
        this.SpecularPower = FindProperty("_SpecularPower", properties);
        this.SpecularColor = FindProperty("_SpecularColor", properties);

        this.BumpMap = FindProperty("_BumpMap", properties);
        this.BumpScale = FindProperty("_BumpScale", properties);

        this.EmissionMap = FindProperty("_EmissionMap", properties);
        this.EmissionColor = FindProperty("_EmissionColor", properties);

        this.queueOffsetProp = FindProperty("_QueueOffset", properties);
    }


    private void DisplayEditor(MaterialEditor materialEditor)
    {
        Material material = materialEditor.target as Material;

        this.DrawSurfaceOptions(material, materialEditor);
        this.DrawToonDiffuseOptions(material, materialEditor);
        this.DrawToonRimOptions(material, materialEditor);
        this.DrawToonSpecularOptions(material, materialEditor);
        this.DrawNormalOptions(material, materialEditor);
        this.DrawToonEmissionOptions(material, materialEditor);
        this.DrawAdvancedOptions(material, materialEditor);
    }

    private void DrawSurfaceOptions(Material material, MaterialEditor materialEditor)
    {
        this.surfaceOptionFoldout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.surfaceOptionFoldout.GetValue(), "Surface Options"));
        if (this.surfaceOptionFoldout.GetValue())
        {
            BaseShaderGUI.DoPopup(ToonShaderEditorStatic.surfaceType, surfaceTypeProp, Enum.GetNames(typeof(BaseShaderGUI.SurfaceType)), materialEditor);
            if ((BaseShaderGUI.SurfaceType) material.GetFloat("_Surface") == BaseShaderGUI.SurfaceType.Transparent)
                BaseShaderGUI.DoPopup(ToonShaderEditorStatic.blendingMode, blendModeProp, Enum.GetNames(typeof(BlendMode)), materialEditor);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = cullingProp.hasMixedValue;
            var culling = (BaseShaderGUI.RenderFace) cullingProp.floatValue;
            culling = (BaseShaderGUI.RenderFace) EditorGUILayout.EnumPopup(ToonShaderEditorStatic.cullingText, culling);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(ToonShaderEditorStatic.cullingText.text);
                cullingProp.floatValue = (float) culling;
                material.doubleSidedGI = (BaseShaderGUI.RenderFace) cullingProp.floatValue != BaseShaderGUI.RenderFace.Front;
            }

            EditorGUI.showMixedValue = false;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = alphaClipProp.hasMixedValue;
            var alphaClipEnabled = EditorGUILayout.Toggle(ToonShaderEditorStatic.alphaClipText, alphaClipProp.floatValue == 1);
            if (EditorGUI.EndChangeCheck())
                alphaClipProp.floatValue = alphaClipEnabled ? 1 : 0;
            EditorGUI.showMixedValue = false;

            if (alphaClipProp.floatValue == 1)
                materialEditor.ShaderProperty(alphaCutoffProp, ToonShaderEditorStatic.alphaClipThresholdText, 1);

            if (receiveShadowsProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = receiveShadowsProp.hasMixedValue;
                var receiveShadows =
                    EditorGUILayout.Toggle(ToonShaderEditorStatic.receiveShadowText, receiveShadowsProp.floatValue == 1.0f);
                if (EditorGUI.EndChangeCheck())
                    receiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
                EditorGUI.showMixedValue = false;
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawToonDiffuseOptions(Material material, MaterialEditor materialEditor)
    {
        this.toonDiffuseOptionsFoldout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.toonDiffuseOptionsFoldout.GetValue(), "Toon Diffuse Options"));
        if (this.toonDiffuseOptionsFoldout.GetValue())
        {
            materialEditor.TexturePropertySingleLine(ToonShaderEditorStatic.BaseMapText, this.BaseMap);
            materialEditor.ColorProperty(this.BaseColor, ToonShaderEditorStatic.BaseColorText);
            materialEditor.TexturePropertySingleLine(ToonShaderEditorStatic.DiffuseRampText, this.DiffuseRamp);
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawToonRimOptions(Material material, MaterialEditor materialEditor)
    {
        this.toonRimOptionsFoldout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.toonRimOptionsFoldout.GetValue(), "Toon Rim Options"));
        if (this.toonRimOptionsFoldout.GetValue())
        {
            materialEditor.RangeProperty(this.RimPower, ToonShaderEditorStatic.RimPowerText);
            if (material.IsKeywordEnabled(ToonShaderEditorStatic.RIM_LIGHTNING_ENABLED))
            {
                EditorGUI.indentLevel += 1;
                materialEditor.FloatProperty(this.RimOffset, ToonShaderEditorStatic.RomOffsetText);
                materialEditor.TexturePropertySingleLine(ToonShaderEditorStatic.RimMapText, this.RimMap);
                materialEditor.ColorProperty(this.RimColor, ToonShaderEditorStatic.RimColorText);
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawToonSpecularOptions(Material material, MaterialEditor materialEditor)
    {
        this.toonSpecularOptionsFoldout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.toonSpecularOptionsFoldout.GetValue(), "Toon Specular Options"));
        if (this.toonSpecularOptionsFoldout.GetValue())
        {
            materialEditor.TexturePropertySingleLine(ToonShaderEditorStatic.SpecularRampText, this.SpecularRamp);
            if (material.IsKeywordEnabled(ToonShaderEditorStatic.TOON_SPECULAR_ENABLED))
            {
                EditorGUI.indentLevel += 1;
                materialEditor.TexturePropertySingleLine(ToonShaderEditorStatic.SpecularMapText, this.SpecularMap);
                materialEditor.FloatProperty(this.SpecularPower, ToonShaderEditorStatic.SpecularPowerText);
                materialEditor.ColorProperty(this.SpecularColor, ToonShaderEditorStatic.SpecularColorText);
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawNormalOptions(Material material, MaterialEditor materialEditor)
    {
        this.toonNormalOptionsFoldout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.toonNormalOptionsFoldout.GetValue(), "Normal map Options"));
        if (this.toonNormalOptionsFoldout.GetValue())
        {
            BaseShaderGUI.DrawNormalArea(materialEditor, this.BumpMap, this.BumpScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawToonEmissionOptions(Material material, MaterialEditor materialEditor)
    {
        this.toonEmissionOptionsFodlout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.toonEmissionOptionsFodlout.GetValue(), "Toon Emission Options"));
        if (this.toonEmissionOptionsFodlout.GetValue())
        {
            materialEditor.TexturePropertySingleLine(ToonShaderEditorStatic.EmissionMapText, this.EmissionMap);
            if (material.IsKeywordEnabled(ToonShaderEditorStatic.TOON_EMISSION_ENABLED))
            {
                EditorGUI.indentLevel += 1;
                materialEditor.ColorProperty(this.EmissionColor, ToonShaderEditorStatic.EmissionColorText);
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawAdvancedOptions(Material material, MaterialEditor materialEditor)
    {
        this.advancedOptionsFoldout.SetValue(EditorGUILayout.BeginFoldoutHeaderGroup(this.advancedOptionsFoldout.GetValue(), "Advanced Options"));
        if (this.advancedOptionsFoldout.GetValue())
        {
            materialEditor.EnableInstancingField();
            if (queueOffsetProp != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = queueOffsetProp.hasMixedValue;
                var queue = EditorGUILayout.IntSlider(ToonShaderEditorStatic.queueSlider, (int) queueOffsetProp.floatValue, -queueOffsetRange, queueOffsetRange);
                if (EditorGUI.EndChangeCheck())
                    queueOffsetProp.floatValue = queue;
                EditorGUI.showMixedValue = false;
            }

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void UpdateMaterialKeywords(MaterialEditor materialEditor)
    {
        Material material = materialEditor.target as Material;
        material.shaderKeywords = null;

        CoreUtils.SetKeyword(material, ToonShaderEditorStatic.RIM_LIGHTNING_ENABLED, this.RimPower.floatValue >= 0f && this.RimPower.floatValue <= 1f);
        CoreUtils.SetKeyword(material, ToonShaderEditorStatic.TOON_SPECULAR_ENABLED, this.SpecularRamp.textureValue != null);
        CoreUtils.SetKeyword(material, ToonShaderEditorStatic.TOON_EMISSION_ENABLED, this.EmissionMap.textureValue != null);

        // Receive Shadows
        if (material.HasProperty("_ReceiveShadows"))
            CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", material.GetFloat("_ReceiveShadows") == 0.0f);

        // Normal Maps
        if (material.HasProperty("_BumpMap"))
            CoreUtils.SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap"));

        BaseShaderGUI.SetupMaterialBlendMode(material);
    }
}

public static class ToonShaderEditorStatic
{
    public static readonly GUIContent surfaceType = new GUIContent("Surface Type",
        "Select a surface type for your texture. Choose between Opaque or Transparent.");

    public static readonly GUIContent blendingMode = new GUIContent("Blending Mode",
        "Controls how the color of the Transparent surface blends with the Material color in the background.");

    public static readonly GUIContent cullingText = new GUIContent("Render Face",
        "Specifies which faces to cull from your geometry. Front culls front faces. Back culls backfaces. None means that both sides are rendered.");

    public static readonly GUIContent alphaClipText = new GUIContent("Alpha Clipping",
        "Makes your Material act like a Cutout shader. Use this to create a transparent effect with hard edges between opaque and transparent areas.");

    public static readonly GUIContent alphaClipThresholdText = new GUIContent("Threshold",
        "Sets where the Alpha Clipping starts. The higher the value is, the brighter the  effect is when clipping starts.");

    public static readonly GUIContent receiveShadowText = new GUIContent("Receive Shadows",
        "When enabled, other GameObjects can cast shadows onto this GameObject.");

    public static readonly GUIContent queueSlider = new GUIContent("Priority",
        "Determines the chronological rendering order for a Material. High values are rendered first.");

    public static string BaseColorText = "Base color";
    public static GUIContent BaseMapText = new GUIContent("Base map");
    public static GUIContent DiffuseRampText = new GUIContent("Diffuse ramp");

    public static string RIM_LIGHTNING_ENABLED = "RIM_LIGHTNING_ENABLED";
    public static string RimPowerText = "Rim power";
    public static GUIContent RimMapText = new GUIContent("Rim map");
    public static string RomOffsetText = "Rim offset";
    public static string RimColorText = "Rim color";

    public static string TOON_SPECULAR_ENABLED = "TOON_SPECULAR_ENABLED";
    public static GUIContent SpecularRampText = new GUIContent("Specular ramp");
    public static GUIContent SpecularMapText = new GUIContent("Specular map");
    public static string SpecularPowerText = "Specular power";
    public static string SpecularColorText = "Specular color";
    
    public static string TOON_EMISSION_ENABLED = "TOON_EMISSION_ENABLED";
    public static GUIContent EmissionMapText = new GUIContent("Emission map");
    public static string EmissionColorText = "Emission color";
}