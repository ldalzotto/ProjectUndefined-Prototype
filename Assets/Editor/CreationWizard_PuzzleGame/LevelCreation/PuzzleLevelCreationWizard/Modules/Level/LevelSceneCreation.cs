using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using Editor_LevelCreation;
using Editor_MainGameCreationWizard;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
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
                PrefabUtility.InstantiatePrefab(editorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.PuzzleDebugModule);
                PrefabUtility.InstantiatePrefab(puzzleLevelDynamicsCreation.CreatedPrefab);

                this.SaveScene(scenePath);
                editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { this.CreatedSceneAsset });
            }
        }

    }


}
