using System;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [Serializable]
    public class LevelSceneConfigurationCreation : ALevelSceneConfigurationCreation //CreateableScriptableObjectComponent<LevelZonesSceneConfigurationData>
    {
        /*
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelZoneConfiguration = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.LevelZoneSceneConfigurationDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelSceneConfigurationData, editorProfile);

            editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration.SetEntry(editorInformationsData.LevelZonesID, levelZoneConfiguration);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration, editorInformationsData.LevelZonesID, levelZoneConfiguration);

        }

        public override void AfterGeneration(AbstractCreationWizardEditorProfile editorProfile)
        {
            var sceneCreation = editorProfile.GetModule<LevelSceneCreation>();
            SerializableObjectHelper.Modify(this.CreatedObject, (so) => { so.FindProperty(nameof(this.CreatedObject.scene)).objectReferenceValue = sceneCreation.CreatedSceneAsset; });
        }
        */

        protected override LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.LevelZonesID;
        }

        protected override CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.CommonGameConfigurations;
        }

        protected override SceneAsset GetCreatedSceneAsset(AbstractCreationWizardEditorProfile editorProfile)
        {
            var sceneCreation = editorProfile.GetModule<LevelSceneCreation>();
            return sceneCreation.CreatedSceneAsset;
        }
    }
}