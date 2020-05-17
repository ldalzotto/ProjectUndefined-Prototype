using CoreGame;
using GameConfigurationID;
using LevelManagement;

namespace Editor_LevelChunkCreationWizard
{
    [System.Serializable]
    public class LevelHierarchyModification : ModificationComponent
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.UpdateLevelHierarchyConfiguration(editorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>(), editorInformationsData.AssociatedAdventureLevelID, editorInformationsData.LevelZoneChunkID, ref editorProfile);
            this.UpdateLevelHierarchyConfiguration(editorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>(), editorInformationsData.AssociatedPuzzleLevelID, editorInformationsData.LevelZoneChunkID, ref editorProfile);
        }

        private void UpdateLevelHierarchyConfiguration(LevelHierarchyConfiguration LevelHierarchyConfiguration, LevelZonesID levelZonesID, LevelZoneChunkID levelZoneChunkID, ref AbstractCreationWizardEditorProfile editorProfile)
        {
            if (levelZonesID != LevelZonesID.NONE)
            {
                LevelHierarchyConfiguration.AddPuzzleChunkLevel(levelZonesID, levelZoneChunkID);
                editorProfile.LevelHierarchyAdded(LevelHierarchyConfiguration, levelZonesID, levelZoneChunkID);
            }
        }
    }
}