using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class PuzzleLevelDynamicsCreation : CreateablePrefabComponent<LevelManager>
    {
        public override Func<AbstractCreationWizardEditorProfile, LevelManager> BasePrefabProvider
        {
            get { return (AbstractCreationWizardEditorProfile editorProfile) => { return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics; }; }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdLevelManager = this.Create(InstancePath.PuzzleLevelDynamicsPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.PuzzleLevelDynamics, editorProfile);
            createdLevelManager.LevelID = editorInformationsData.LevelZonesID;
            PrefabUtility.SavePrefabAsset(createdLevelManager.gameObject);
        }
    }
}