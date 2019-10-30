using System;
using System.Collections.Generic;
using CreationWizard;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;
using UnityEngine;

namespace Editor_PuzzleLevelCreationWizard
{
    [Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        [SerializeField] public EditorInformationsData EditorInformationsData;

        protected override string headerDescriptionLabel => "Base informations used by the creation wizard.";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        private void InitProperties()
        {
            EditorInformationsHelper.InitProperties(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeErrorState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>()
                {
                    ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZonesID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelConfiguration>()),
                    ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZonesID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelZonesSceneConfiguration>()),
                    ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZonesID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>()),
                    ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedAdventureLevelID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>().GetKeys(), typeof(LevelHierarchyConfiguration).Name)
                }
                .Find((s) => !string.IsNullOrEmpty(s));
        }
    }

    [Serializable]
    public class EditorInformationsData
    {
        [CustomEnum()] public LevelZonesID LevelZonesID;
        [CustomEnum()] public LevelZonesID AssociatedAdventureLevelID;

        public CommonGameConfigurations CommonGameConfigurations;
    }
}