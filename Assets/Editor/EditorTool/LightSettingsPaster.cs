using ConfigurationEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.LightmapEditorSettings;

public class LightSettingsPaster : EditorWindow
{
    [MenuItem("EditorTool/LightSettingsPaster")]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(LightSettingsPaster));
    }

    private void OnEnable()
    {
        this.targetScenes = new List<SceneAssetWrapper>();
        this.targetScenesGUI = new FoldableReordableList<SceneAssetWrapper>(this.targetScenes, true, false, true, true, "Target Scenes", 1f, this.DoSingleTargetScene);
    }

    private void OnGUI()
    {
        this.DoSourceScene();
        this.DoTargetScenes();
        this.DoPasting();
    }

    private SceneAsset sourceScene;

    private void DoSourceScene()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Source Scene : ");
        this.sourceScene = (SceneAsset)EditorGUILayout.ObjectField(this.sourceScene, typeof(SceneAsset), false);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();
    }

    private List<SceneAssetWrapper> targetScenes;
    private FoldableReordableList<SceneAssetWrapper> targetScenesGUI;

    private void DoTargetScenes()
    {
        EditorGUILayout.BeginVertical();
        this.targetScenesGUI.DoLayoutList();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();
    }

    private void DoSingleTargetScene(Rect rect, int index, bool isActive, bool isFocused)
    {
        this.targetScenes[index].SceneAsset = (SceneAsset)EditorGUI.ObjectField(rect, this.targetScenes[index].SceneAsset, typeof(SceneAsset), false);
    }

    private LightmapEditorSettingsWrapper sourceLightmapEditorSettingsWrapper;

    private void DoPasting()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("PASTE"))
        {
            var sourceScene = AssetFinder.SafeSingleAssetFind<SceneAsset>(this.sourceScene.name + " t:Scene");
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sourceScene), OpenSceneMode.Single);

            this.sourceLightmapEditorSettingsWrapper = this.ExtractLightmapEditorSettingsFromActiveScene();

            foreach (var targetScene in this.targetScenes)
            {
                var targetSceneLoaded = AssetFinder.SafeSingleAssetFind<SceneAsset>(targetScene.SceneAsset.name + " t:Scene");
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(targetSceneLoaded), OpenSceneMode.Single);
                this.PasteLightMapEditorSettingsFromWrapper(this.sourceLightmapEditorSettingsWrapper);
                // EditorSceneManager.MarkSceneDirty()
                EditorSceneManager.MarkAllScenesDirty();
                EditorSceneManager.SaveOpenScenes();
            }

        }
        EditorGUILayout.EndVertical();
    }

    [System.Serializable]
    class SceneAssetWrapper
    {
        public SceneAsset SceneAsset;

        public SceneAssetWrapper()
        {
        }
    }

    private LightmapEditorSettingsWrapper ExtractLightmapEditorSettingsFromActiveScene()
    {
        var LightmapEditorSettingsWrapper = new LightmapEditorSettingsWrapper();

        LightmapEditorSettingsWrapper.lightmapper = LightmapEditorSettings.lightmapper;
        LightmapEditorSettingsWrapper.aoMaxDistance = LightmapEditorSettings.aoMaxDistance;
        LightmapEditorSettingsWrapper.aoExponentDirect = LightmapEditorSettings.aoExponentDirect;
        LightmapEditorSettingsWrapper.padding = LightmapEditorSettings.padding;
        LightmapEditorSettingsWrapper.enableAmbientOcclusion = LightmapEditorSettings.enableAmbientOcclusion;
        LightmapEditorSettingsWrapper.resolution = LightmapEditorSettings.realtimeResolution;
        LightmapEditorSettingsWrapper.giPathTracerSampling = LightmapEditorSettings.sampling;
        LightmapEditorSettingsWrapper.maxAtlasWidth = LightmapEditorSettings.maxAtlasSize;
        LightmapEditorSettingsWrapper.reflectionCubemapCompression = LightmapEditorSettings.reflectionCubemapCompression;
        LightmapEditorSettingsWrapper.aoExponentIndirect = LightmapEditorSettings.aoExponentIndirect;
        LightmapEditorSettingsWrapper.bakeResolution = LightmapEditorSettings.bakeResolution;
        LightmapEditorSettingsWrapper.lightmapsMode = LightmapEditorSettings.lightmapsMode;
        LightmapEditorSettingsWrapper.mixedBakeMode = LightmapEditorSettings.mixedBakeMode;
        LightmapEditorSettingsWrapper.sampling = LightmapEditorSettings.sampling;
        LightmapEditorSettingsWrapper.directSampleCount = LightmapEditorSettings.directSampleCount;
        LightmapEditorSettingsWrapper.indirectSampleCount = LightmapEditorSettings.indirectSampleCount;
        LightmapEditorSettingsWrapper.bounces = LightmapEditorSettings.bounces;
        LightmapEditorSettingsWrapper.prioritizeView = LightmapEditorSettings.prioritizeView;
        LightmapEditorSettingsWrapper.filteringMode = LightmapEditorSettings.filteringMode;
        LightmapEditorSettingsWrapper.textureCompression = LightmapEditorSettings.textureCompression;
        LightmapEditorSettingsWrapper.denoiserTypeIndirect = LightmapEditorSettings.denoiserTypeIndirect;
        LightmapEditorSettingsWrapper.denoiserTypeAO = LightmapEditorSettings.denoiserTypeAO;
        LightmapEditorSettingsWrapper.denoiserTypeDirect = LightmapEditorSettings.denoiserTypeDirect;
        LightmapEditorSettingsWrapper.filterTypeIndirect = LightmapEditorSettings.filterTypeIndirect;
        LightmapEditorSettingsWrapper.filterTypeAO = LightmapEditorSettings.filterTypeAO;
        LightmapEditorSettingsWrapper.filteringGaussRadiusDirect = LightmapEditorSettings.filteringGaussRadiusDirect;
        LightmapEditorSettingsWrapper.filteringGaussRadiusIndirect = LightmapEditorSettings.filteringGaussRadiusIndirect;
        LightmapEditorSettingsWrapper.filteringGaussRadiusAO = LightmapEditorSettings.filteringGaussRadiusAO;
        LightmapEditorSettingsWrapper.filteringAtrousPositionSigmaDirect = LightmapEditorSettings.filteringAtrousPositionSigmaDirect;
        LightmapEditorSettingsWrapper.filteringAtrousPositionSigmaIndirect = LightmapEditorSettings.filteringAtrousPositionSigmaIndirect;
        LightmapEditorSettingsWrapper.filteringAtrousPositionSigmaAO = LightmapEditorSettings.filteringAtrousPositionSigmaAO;
        LightmapEditorSettingsWrapper.filterTypeDirect = LightmapEditorSettings.filterTypeDirect;
        LightmapEditorSettingsWrapper.realtimeResolution = LightmapEditorSettings.realtimeResolution;


        return LightmapEditorSettingsWrapper;
    }

    private void PasteLightMapEditorSettingsFromWrapper(LightmapEditorSettingsWrapper LightmapEditorSettingsWrapper)
    {
        LightmapEditorSettings.lightmapper = LightmapEditorSettingsWrapper.lightmapper;
        LightmapEditorSettings.aoMaxDistance = LightmapEditorSettingsWrapper.aoMaxDistance;
        LightmapEditorSettings.aoExponentDirect = LightmapEditorSettingsWrapper.aoExponentDirect;
        LightmapEditorSettings.padding = LightmapEditorSettingsWrapper.padding;
        LightmapEditorSettings.enableAmbientOcclusion = LightmapEditorSettingsWrapper.enableAmbientOcclusion;
        LightmapEditorSettings.realtimeResolution = LightmapEditorSettingsWrapper.resolution;
        LightmapEditorSettings.sampling = LightmapEditorSettingsWrapper.giPathTracerSampling;
        LightmapEditorSettings.maxAtlasSize = LightmapEditorSettingsWrapper.maxAtlasWidth;
        LightmapEditorSettings.reflectionCubemapCompression = LightmapEditorSettingsWrapper.reflectionCubemapCompression;
        LightmapEditorSettings.aoExponentIndirect = LightmapEditorSettingsWrapper.aoExponentIndirect;
        LightmapEditorSettings.bakeResolution = LightmapEditorSettingsWrapper.bakeResolution;
        LightmapEditorSettings.lightmapsMode = LightmapEditorSettingsWrapper.lightmapsMode;
        LightmapEditorSettings.mixedBakeMode = LightmapEditorSettingsWrapper.mixedBakeMode;
        LightmapEditorSettings.sampling = LightmapEditorSettingsWrapper.sampling;
        LightmapEditorSettings.directSampleCount = LightmapEditorSettingsWrapper.directSampleCount;
        LightmapEditorSettings.indirectSampleCount = LightmapEditorSettingsWrapper.indirectSampleCount;
        LightmapEditorSettings.bounces = LightmapEditorSettingsWrapper.bounces;
        LightmapEditorSettings.prioritizeView = LightmapEditorSettingsWrapper.prioritizeView;
        LightmapEditorSettings.filteringMode = LightmapEditorSettingsWrapper.filteringMode;
        LightmapEditorSettings.textureCompression = LightmapEditorSettingsWrapper.textureCompression;
        LightmapEditorSettings.denoiserTypeIndirect = LightmapEditorSettingsWrapper.denoiserTypeIndirect;
        LightmapEditorSettings.denoiserTypeAO = LightmapEditorSettingsWrapper.denoiserTypeAO;
        LightmapEditorSettings.denoiserTypeDirect = LightmapEditorSettingsWrapper.denoiserTypeDirect;
        LightmapEditorSettings.filterTypeIndirect = LightmapEditorSettingsWrapper.filterTypeIndirect;
        LightmapEditorSettings.filterTypeAO = LightmapEditorSettingsWrapper.filterTypeAO;
        LightmapEditorSettings.filteringGaussRadiusDirect = LightmapEditorSettingsWrapper.filteringGaussRadiusDirect;
        LightmapEditorSettings.filteringGaussRadiusIndirect = LightmapEditorSettingsWrapper.filteringGaussRadiusIndirect;
        LightmapEditorSettings.filteringGaussRadiusAO = LightmapEditorSettingsWrapper.filteringGaussRadiusAO;
        LightmapEditorSettings.filteringAtrousPositionSigmaDirect = LightmapEditorSettingsWrapper.filteringAtrousPositionSigmaDirect;
        LightmapEditorSettings.filteringAtrousPositionSigmaIndirect = LightmapEditorSettingsWrapper.filteringAtrousPositionSigmaIndirect;
        LightmapEditorSettings.filteringAtrousPositionSigmaAO = LightmapEditorSettingsWrapper.filteringAtrousPositionSigmaAO;
        LightmapEditorSettings.filterTypeDirect = LightmapEditorSettingsWrapper.filterTypeDirect;
        LightmapEditorSettings.realtimeResolution = LightmapEditorSettingsWrapper.realtimeResolution;
    }

    [System.Serializable]
    class LightmapEditorSettingsWrapper
    {
        public Lightmapper lightmapper;
        public float aoMaxDistance;
        public float aoExponentDirect;
        public int padding;
        public bool enableAmbientOcclusion;
        public float resolution;
        public LightmapEditorSettings.Sampling giPathTracerSampling;
        public int maxAtlasWidth;
        public ReflectionCubemapCompression reflectionCubemapCompression;
        public float aoExponentIndirect;
        public float bakeResolution;
        public LightmapsMode lightmapsMode;
        public MixedLightingMode mixedBakeMode;
        public Sampling sampling;
        public int directSampleCount;
        public int indirectSampleCount;
        public int bounces;
        public bool prioritizeView;
        public LightmapEditorSettings.FilterMode filteringMode;
        public bool textureCompression;
        public DenoiserType denoiserTypeIndirect;
        public DenoiserType denoiserTypeAO;
        public DenoiserType denoiserTypeDirect;
        public FilterType filterTypeIndirect;
        public FilterType filterTypeAO;
        public int filteringGaussRadiusDirect;
        public int filteringGaussRadiusIndirect;
        public int filteringGaussRadiusAO;
        public float filteringAtrousPositionSigmaDirect;
        public float filteringAtrousPositionSigmaIndirect;
        public float filteringAtrousPositionSigmaAO;
        public FilterType filterTypeDirect;
        public float realtimeResolution;
    }
}
