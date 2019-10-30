using System;
using System.Collections.Generic;
using Editor_MainGameCreationWizard;
using LevelManagement;
using UnityEditor;

[Serializable]
public abstract class ALevelHierarchyCreation : CreateableScriptableObjectComponent<LevelHierarchyConfigurationData>
{
    protected abstract LevelZonesID GetLevelZonesID(AbstractCreationWizardEditorProfile editorProfile);
    protected abstract CommonGameConfigurations GetCommonGameConfigurations(AbstractCreationWizardEditorProfile editorProfile);

    public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
    {
        var generatedHierarchy = this.CreateAsset(InstancePath.PuzzleLevelHierarchyDataPath, this.GetLevelZonesID(editorProfile) + NameConstants.LevelHierarchyConfigurationData, editorProfile);

        this.GetCommonGameConfigurations(editorProfile).GetConfiguration<LevelHierarchyConfiguration>().SetEntry(this.GetLevelZonesID(editorProfile), generatedHierarchy);
        editorProfile.GameConfigurationModified(this.GetCommonGameConfigurations(editorProfile).GetConfiguration<LevelHierarchyConfiguration>(), this.GetLevelZonesID(editorProfile), generatedHierarchy);

        var generatedHierarchySerialized = new SerializedObject(generatedHierarchy);
        SerializableObjectHelper.SetArray((new List<LevelZoneChunkID>()).ConvertAll(e => (Enum) e), generatedHierarchySerialized.FindProperty(nameof(generatedHierarchy.LevelHierarchy)));
        generatedHierarchySerialized.ApplyModifiedProperties();
    }
}