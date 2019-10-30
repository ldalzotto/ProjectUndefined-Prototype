using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_PuzzleLevelCreationWizard
{
    [Serializable]
    [CreateAssetMenu(fileName = "PuzzleLevelCreationWizardEditorProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/PuzzleLevelCreationWizardEditorProfile", order = 1)]
    public class PuzzleLevelCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>()
        {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(PuzzleLevelDynamicsCreation), 3),
            new CreationWizardOrderConfiguration(typeof(LevelHierarchyCreation), 4),
            new CreationWizardOrderConfiguration(typeof(LevelSceneConfigurationCreation), 5, 0),
            new CreationWizardOrderConfiguration(typeof(LevelSceneCreation), 6),
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}