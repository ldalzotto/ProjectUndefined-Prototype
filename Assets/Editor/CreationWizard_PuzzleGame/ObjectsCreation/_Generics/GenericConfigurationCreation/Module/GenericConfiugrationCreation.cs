using Editor_MainGameCreationWizard;
using UnityEditor;
using UnityEngine;

namespace Editor_GenericConfigurationCreation
{
    [System.Serializable]
    public class GenericConfiugrationCreation : CreationModuleComponent
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;

            var inherentDataClass = editorInfomrationsData.SelectedConfiguration.GetType().BaseType.GetGenericArguments()[1];

            var generateSOObjectManager = new GeneratedScriptableObjectManager<ScriptableObject>(ScriptableObject.CreateInstance(inherentDataClass),
                    InstancePath.GetConfigurationDataPath((ScriptableObject)editorInfomrationsData.SelectedConfiguration),
                     editorInfomrationsData.SelectedKey.ToString() + "_" + editorInfomrationsData.SelectedConfiguration.GetType().BaseType.GetGenericArguments()[0].Name);

            editorProfile.AddToGeneratedObjects(generateSOObjectManager.GeneratedAsset);

            editorInfomrationsData.SelectedConfiguration.SetEntry(editorInfomrationsData.SelectedKey, generateSOObjectManager.GeneratedAsset);
            EditorUtility.SetDirty((UnityEngine.Object)editorInfomrationsData.SelectedConfiguration);
            editorProfile.GameConfigurationModified((UnityEngine.Object)editorInfomrationsData.SelectedConfiguration, editorInfomrationsData.SelectedKey, generateSOObjectManager.GeneratedAsset);
        }

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
        }
    }
}