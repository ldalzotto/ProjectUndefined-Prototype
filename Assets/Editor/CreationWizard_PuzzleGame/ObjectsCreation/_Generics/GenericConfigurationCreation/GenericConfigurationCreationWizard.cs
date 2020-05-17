using System;

namespace Editor_GenericConfigurationCreation
{
    public class GenericConfigurationCreationWizard : AbstractCreationWizardEditor<GenericConfigurationCreationWizardProfile>
    {
        public void SetSelectedConfiguration(Type configurationType)
        {
            var EditorInformations = this.editorProfile.GetModule<EditorInformations>();
            EditorInformations.SetSelectedConf(configurationType);
        }
    }

}
