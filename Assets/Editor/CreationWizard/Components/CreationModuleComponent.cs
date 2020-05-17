using OdinSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class CreationModuleComponent : SerializedScriptableObject
{
    [HideInInspector]
    public bool ModuleFoldout;

    [HideInInspector]
    public string TmpFolderPath;

    private string warningMessage;
    private string errorMessage;
    private GUIStyle foldoutStyle;


    protected virtual string foldoutLabel { get; }
    protected virtual string headerDescriptionLabel { get; }

    public static CreationModuleComponent Create(Type objType, string filePath, string tmpFolderPath)
    {
        var instance = Create(objType, filePath);
        instance.TmpFolderPath = tmpFolderPath;
        return instance;
    }

    public static CreationModuleComponent Create(Type objType, string filePath)
    {
        var instance = (CreationModuleComponent)ScriptableObject.CreateInstance(objType);
        instance.ModuleFoldout = false;
        AssetDatabase.CreateAsset(instance, filePath);
        return instance;
    }

    public void OnInspectorGUI(AbstractCreationWizardEditorProfile editorProfile)
    {
        this.DoInit();

        if (!string.IsNullOrEmpty(this.errorMessage))
        {
            this.SetFoldoutStyleTextColor(ref this.foldoutStyle, Color.red);
        }
        else if (!string.IsNullOrEmpty(this.warningMessage))
        {
            this.SetFoldoutStyleTextColor(ref this.foldoutStyle, Color.yellow);
        }
        else
        {
            this.SetFoldoutStyleTextColor(ref this.foldoutStyle, Color.black);
        }

        this.warningMessage = this.ComputeWarningState(editorProfile);
        this.errorMessage = this.ComputeErrorState(editorProfile);
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.BeginHorizontal();

        var displayedFoldoutLabel = this.foldoutLabel;
        if (displayedFoldoutLabel == null)
        {
            displayedFoldoutLabel = this.GetType().Name + " : ";
        }
        this.ModuleFoldout = EditorGUILayout.Foldout(this.ModuleFoldout, displayedFoldoutLabel, true, this.foldoutStyle);

        EditorGUILayout.EndHorizontal();

        if (this.ModuleFoldout)
        {
            var serializedObject = new SerializedObject(this);
            EditorGUILayout.LabelField(this.headerDescriptionLabel, EditorStyles.miniLabel);

            this.OnInspectorGUIImpl(serializedObject, editorProfile);
            serializedObject.ApplyModifiedProperties();

            if (!string.IsNullOrEmpty(this.errorMessage))
            {
                EditorGUILayout.HelpBox(this.errorMessage, MessageType.Error, true);
            }
            else if (!string.IsNullOrEmpty(this.warningMessage))
            {
                EditorGUILayout.HelpBox(this.warningMessage, MessageType.Warning, true);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DoInit()
    {
        if (this.foldoutStyle == null)
        {
            this.foldoutStyle = new GUIStyle(EditorStyles.foldout);
        }
    }

    public virtual void AfterGeneration(AbstractCreationWizardEditorProfile editorProfile) { }
    protected abstract void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile);
    public abstract void ResetEditor();
    public virtual string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile) { return string.Empty; }
    public virtual string ComputeErrorState(AbstractCreationWizardEditorProfile editorProfile) { return string.Empty; }

    public virtual void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile) { }
    public virtual void OnGenerationEnd() { }
    private void SetFoldoutStyleTextColor(ref GUIStyle style, Color textColor)
    {
        style.normal.textColor = textColor;
        style.onNormal.textColor = textColor;
        style.hover.textColor = textColor;
        style.onHover.textColor = textColor;
        style.focused.textColor = textColor;
        style.onFocused.textColor = textColor;
        style.active.textColor = textColor;
        style.onActive.textColor = textColor;
    }

    public bool HasWarning()
    {
        return !string.IsNullOrEmpty(this.warningMessage);
    }

    public bool HasError()
    {
        return !string.IsNullOrEmpty(this.errorMessage);
    }

    protected void CreateFolderIfNecessary(string directoryPath)
    {
        var di = new DirectoryInfo(directoryPath);
        if (!di.Exists)
        {
            di.Create();
        }
    }

}
