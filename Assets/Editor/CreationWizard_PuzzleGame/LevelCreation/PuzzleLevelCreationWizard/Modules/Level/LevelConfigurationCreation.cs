using System;
using Editor_MainGameCreationWizard;
using LevelManagement;

namespace Editor_PuzzleLevelCreationWizard
{
    [Serializable]
    public class LevelConfigurationCreation : CreateableScriptableObjectComponent<LevelConfigurationData>
    {
        protected override string objectFieldLabel => typeof(LevelConfigurationData).Name;

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelConfiguration = editorInformationsData.CommonGameConfigurations.GetConfiguration<LevelConfiguration>();
            var createdAsset = this.CreateAsset(InstancePath.GetConfigurationDataPath(levelConfiguration), editorInformationsData.LevelZonesID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            levelConfiguration.SetEntry(editorInformationsData.LevelZonesID, createdAsset);
            editorProfile.GameConfigurationModified(editorInformationsData.CommonGameConfigurations.GetConfiguration<LevelConfiguration>(), editorInformationsData.LevelZonesID, createdAsset);
        }
    }
}