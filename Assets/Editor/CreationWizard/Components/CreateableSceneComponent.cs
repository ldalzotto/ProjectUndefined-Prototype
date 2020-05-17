using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public abstract class CreateableSceneComponent : CreationModuleComponent
{
    [SerializeField]
    private SceneAsset createdSceneAsset;

    private Scene tmpCreatedScene;

    protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("createdSceneAsset"));
    }

    public SceneAsset CreatedSceneAsset { get => createdSceneAsset; }

    public override void ResetEditor()
    {
        this.createdSceneAsset = null;
    }

    protected void CreateNewScene()
    {
        this.tmpCreatedScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
    }

    protected bool SaveScene(string path)
    {
        var sceneSaved = EditorSceneManager.SaveScene(this.tmpCreatedScene, path);
        this.createdSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        this.AddSceneToBuilIfNecessary(path);
        return sceneSaved;
    }


    private void AddSceneToBuilIfNecessary(string newScenePath)
    {
        bool addToBuild = true;
        foreach (var buildedScene in EditorBuildSettings.scenes)
        {
            if (buildedScene.path == newScenePath)
            {
                addToBuild = false;
            }
        }
        if (addToBuild)
        {
            var newSceneList = EditorBuildSettings.scenes.ToList();
            newSceneList.Add(new EditorBuildSettingsScene(newScenePath, true));
            EditorBuildSettings.scenes = newSceneList.ToArray();
        }
    }

}
