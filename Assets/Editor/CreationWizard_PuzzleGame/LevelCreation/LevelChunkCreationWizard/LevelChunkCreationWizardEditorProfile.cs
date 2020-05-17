using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Editor_LevelChunkCreationWizard
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelChunkCreationWizardEditorProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/LevelChunkCreationWizardEditorProfile", order = 1)]
    public class LevelChunkCreationWizardEditorProfile : AbstractCreationWizardEditorProfile
    {

        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>()
        {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(LevelChunkPrefabCreation), 0),
            new CreationWizardOrderConfiguration(typeof(LevelChunkConfigurationCreation), 1, 0),
            new CreationWizardOrderConfiguration(typeof(LevelHierarchyModification), 2),
            new CreationWizardOrderConfiguration(typeof(LevelChunkSceneCreation), 2),
        };
        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }

}
