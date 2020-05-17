using System;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;

namespace Editor_LevelChunkCreationWizard
{
    [Serializable]
    public class LevelChunkPrefabCreation : CreateablePrefabComponent<LevelChunkInteractiveObjectInitializer>
    {
        public override Func<AbstractCreationWizardEditorProfile, LevelChunkInteractiveObjectInitializer> BasePrefabProvider
        {
            get { return (AbstractCreationWizardEditorProfile editorProfile) => { return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab; }; }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdBaseChunk = this.Create(InstancePath.LevelChunkBaseLevelPrefabPath, editorInformationsData.LevelZoneChunkID.ToString() + NameConstants.BaseLevelChunkPrefab, editorProfile);
            createdBaseChunk.LevelChunkInteractiveObjectDefinition.LevelZoneChunkID = editorInformationsData.LevelZoneChunkID;
            PrefabUtility.SavePrefabAsset(createdBaseChunk.gameObject);
        }
    }
}