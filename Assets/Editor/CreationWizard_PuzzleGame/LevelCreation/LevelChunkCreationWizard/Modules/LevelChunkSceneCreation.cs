using Editor_LevelCreation;
using Editor_MainGameCreationWizard;
using UnityEditor;
using UnityEngine;

namespace Editor_LevelChunkCreationWizard
{
    [System.Serializable]
    public class LevelChunkSceneCreation : CreateableSceneComponent
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var levelChunkPrefabCreation = editorProfile.GetModule<LevelChunkPrefabCreation>();
            this.CreateNewScene();
            var scenePath = LevelPathHelper.BuilChunkPath(InstancePath.LevelBasePath, editorInformationsData.AssociatedAdventureLevelID, editorInformationsData.LevelZoneChunkID);
            if (this.SaveScene(scenePath))
            {
                PrefabUtility.InstantiatePrefab(levelChunkPrefabCreation.CreatedPrefab);
                this.SaveScene(scenePath);
                editorProfile.AddToGeneratedObjects(new Object[] { this.CreatedSceneAsset });

            }
        }
    }
}