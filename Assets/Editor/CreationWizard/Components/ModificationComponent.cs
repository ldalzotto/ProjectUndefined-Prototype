using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class ModificationComponent : CreationModuleComponent
{
    protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
    {
    }

    public override void ResetEditor()
    {
    }
}
