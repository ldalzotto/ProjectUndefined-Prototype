using System;
using System.Collections.Generic;
using CreationWizard;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;
using UnityEngine;

namespace Editor_LevelChunkCreationWizard
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
            string associatedPuzzleLevelWargning = string.Empty;
            if (this.EditorInformationsData.AssociatedPuzzleLevelID != LevelZonesID.NONE)
            {
                associatedPuzzleLevelWargning = ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedPuzzleLevelID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>().GetKeys(), typeof(LevelHierarchyConfiguration).Name);
            }

            return new List<string>()
                {
                    associatedPuzzleLevelWargning,
                    ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedAdventureLevelID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>().GetKeys(), typeof(LevelHierarchyConfiguration).Name),
                    ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZoneChunkID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<ChunkZonesSceneConfiguration>()),
                    ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedAdventureLevelID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>().GetKeys(), typeof(LevelHierarchyConfiguration).Name)
                }
                .Find((s) => !string.IsNullOrEmpty(s));
        }
    }

    [Serializable]
    public class EditorInformationsData
    {
        [CustomEnum()] public LevelZoneChunkID LevelZoneChunkID;
        [CustomEnum()] public LevelZonesID AssociatedAdventureLevelID;
        [CustomEnum()] public LevelZonesID AssociatedPuzzleLevelID = LevelZonesID.NONE;

        public CommonGameConfigurations CommonGameConfigurations;
    }
}