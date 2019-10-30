using System;
using Editor_LevelCreation;
using Editor_MainGameCreationWizard;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Editor_PuzzleLevelCreationWizard
{
    [Serializable]
    public class LevelSceneCreation : CreateableSceneComponent
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var puzzleLevelDynamicsCreation = editorProfile.GetModule<PuzzleLevelDynamicsCreation>();
            this.CreateNewScene();
            var scenePath = LevelPathHelper.BuildBaseLevelPath(InstancePath.LevelBasePath, editorInformationsData.AssociatedAdventureLevelID, editorInformationsData.LevelZonesID);
            if (this.SaveScene(scenePath))
            {
                PrefabUtility.InstantiatePrefab(editorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements);
                PrefabUtility.InstantiatePrefab(editorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance);
                PrefabUtility.InstantiatePrefab(puzzleLevelDynamicsCreation.CreatedPrefab);

                this.SaveScene(scenePath);
                editorProfile.AddToGeneratedObjects(new Object[] {this.CreatedSceneAsset});
            }
        }
    }
}