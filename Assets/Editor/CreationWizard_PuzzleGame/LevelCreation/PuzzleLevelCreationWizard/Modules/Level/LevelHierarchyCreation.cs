using System;
using Editor_MainGameCreationWizard;
using LevelManagement;

namespace Editor_PuzzleLevelCreationWizard
{
    [Serializable]
    public class LevelHierarchyCreation : ALevelHierarchyCreation
    {
        protected override CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.CommonGameConfigurations;
        }

        protected override LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            return editorInformationsData.LevelZonesID;
        }
    }
}