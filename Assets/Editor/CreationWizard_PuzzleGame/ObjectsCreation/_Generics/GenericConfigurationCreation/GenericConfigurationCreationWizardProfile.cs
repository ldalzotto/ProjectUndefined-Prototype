using System.Collections.Generic;
using UnityEngine;

namespace Editor_GenericConfigurationCreation
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GenericConfigurationCreationWizardProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile/Generic/GenericConfigurationCreationWizardProfile", order = 1)]
    public class GenericConfigurationCreationWizardProfile : AbstractCreationWizardEditorProfile
    {
        private List<CreationWizardOrderConfiguration> ModuleTypes_IMPL = new List<CreationWizardOrderConfiguration>() {
            new CreationWizardOrderConfiguration(typeof(EditorInformations), -1),
            new CreationWizardOrderConfiguration(typeof(GenericConfiugrationCreation), 0)
        };

        public override List<CreationWizardOrderConfiguration> ModulesConfiguration => this.ModuleTypes_IMPL;
    }
}