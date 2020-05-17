using System;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;

namespace Editor_LevelChunkCreationWizard
{
    [Serializable]
    public class LevelChunkConfigurationCreation : CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.LevelZoneChunkSceneConfigurationDataPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.LevelChunkSceneConfigurationData, editorProfile);
            this.AddToGameConfiguration(editorInformationsData.LevelZoneChunkID, editorInformationsData.CommonGameConfigurations.GetConfiguration<ChunkZonesSceneConfiguration>(), editorProfile);
        }

        public override void AfterGeneration(AbstractCreationWizardEditorProfile editorProfile)
        {
            var levelChunkSceneCreation = editorProfile.GetModule<LevelChunkSceneCreation>();
            var serializedCreatedObject = new SerializedObject(this.CreatedObject);
            serializedCreatedObject.FindProperty(nameof(this.CreatedObject.scene)).objectReferenceValue = levelChunkSceneCreation.CreatedSceneAsset;
            serializedCreatedObject.ApplyModifiedProperties();
        }
    }
}