using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public interface ICreationWizardEditor
{
    void OnGUI();
}

public abstract class AbstractCreationWizardEditor<T> : ICreationWizardEditor where T : AbstractCreationWizardEditorProfile
{
    protected T editorProfile;

    public void OnGUI()
    {
        this.editorProfile = (T)EditorGUILayout.ObjectField(this.editorProfile, typeof(T), false);

        if (this.editorProfile == null)
        {
            this.editorProfile = AssetFinder.SafeSingleAssetFind<T>("t:" + typeof(T).Name);
        }

        if (this.editorProfile != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("R", "Reset creation wzard."), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                this.editorProfile.ResetEditor();
            }
            if (GUILayout.Button(new GUIContent(".", "Collapse all."), EditorStyles.miniButtonMid, GUILayout.Width(20)))
            {
                this.editorProfile.ColapseAll();
            }
            if (GUILayout.Button(new GUIContent("*", "Create all."), EditorStyles.miniButtonMid, GUILayout.Width(20)))
            {
                this.editorProfile.CreateAll();
            }
            EditorGUILayout.EndHorizontal();

            editorProfile.WizardScrollPosition = EditorGUILayout.BeginScrollView(editorProfile.WizardScrollPosition);
            this.OnWizardGUI();

            if (GUILayout.Button("GENERATE"))
            {
                if (this.editorProfile.ContainsError())
                {
                    if (EditorUtility.DisplayDialog("Proceed generation ?", "There are errors. Do you want to proceed generation ?", "YES", "NO"))
                    {
                        DoGeneration();
                    }
                }
                else if (this.editorProfile.ContainsWarn())
                {
                    if (EditorUtility.DisplayDialog("Proceed generation ?", "There are warnings. Do you want to proceed generation ?", "YES", "NO"))
                    {
                        DoGeneration();
                    }
                }
                else
                {
                    DoGeneration();
                }
            }

            this.DoGenereatedObject();
            EditorGUILayout.EndScrollView();

        }
    }

    private void DoGeneration()
    {
        this.editorProfile.CreationWizardFeedLines.Clear();

        var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();

        try
        {
            var modulesToGenerate = new List<CreationWizardOrderConfiguration>(this.editorProfile.ModulesConfiguration).Select(c => c).Where(c => c.GenerationOrder != -1).ToList();
            modulesToGenerate.Sort((c1, c2) =>
                {
                    return c1.GenerationOrder.CompareTo(c2.GenerationOrder);
                });

            foreach (var module in modulesToGenerate)
            {
                this.editorProfile.Modules[module.ModuleType.Name].OnGenerationClicked(this.editorProfile);
            }


            var afterGenerationModules = new List<CreationWizardOrderConfiguration>(this.editorProfile.ModulesConfiguration).Select(c => c).Where(c => c.AfterGenerationOrder != -1).ToList();
            afterGenerationModules.Sort((c1, c2) =>
            {
                return c1.AfterGenerationOrder.CompareTo(c2.AfterGenerationOrder);
            });

            foreach (var module in afterGenerationModules)
            {
                this.editorProfile.Modules[module.ModuleType.Name].AfterGeneration(this.editorProfile);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e.StackTrace);
        }
        finally
        {
            this.editorProfile.OnGenerationEnd();
            EditorSceneManager.CloseScene(tmpScene, true);
        }
    }

    private void OnWizardGUI()
    {
        foreach (var module in this.editorProfile.Modules.Values)
        {
            module.OnInspectorGUI(this.editorProfile);
        }
    }

    private void DoGenereatedObject()
    {
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Generated objects : ");
        if (GUILayout.Button(new GUIContent("D", "Delete all generations."), EditorStyles.miniButton, GUILayout.Width(20)))
        {

            foreach (var creationWizardFeedLines in this.editorProfile.CreationWizardFeedLines)
            {
                if (creationWizardFeedLines.GetType() == typeof(CreatedObjectFeedLine))
                {
                    var feedLine = (CreatedObjectFeedLine)creationWizardFeedLines;
                    AssetDatabase.DeleteAsset(feedLine.FilePath);
                }
                else if (creationWizardFeedLines.GetType() == typeof(ConfigurationModifiedFeedLine))
                {
                    ((ConfigurationModifiedFeedLine)creationWizardFeedLines).RemoveEntry();
                }
                else if (creationWizardFeedLines.GetType() == typeof(LevelHierarchyAddFeedLine))
                {
                    var LevelHierarchyAddFeedLine = (LevelHierarchyAddFeedLine)creationWizardFeedLines;
                    LevelHierarchyAddFeedLine.LevelHierarchyConfiguration.RemovePuzzleChunkLevel(LevelHierarchyAddFeedLine.LevelZonesID, LevelHierarchyAddFeedLine.AddedChunkID);
                }
            }
            this.editorProfile.CreationWizardFeedLines.Clear();
        }
        EditorGUILayout.EndHorizontal();
        foreach (var creationWizardFeedLines in this.editorProfile.CreationWizardFeedLines)
        {
            creationWizardFeedLines.GUITick();
        }
        EditorGUILayout.EndVertical();
    }

    protected M GetModule<M>() where M : CreationModuleComponent
    {
        return (M)this.editorProfile.Modules[typeof(M).Name];
    }
}
