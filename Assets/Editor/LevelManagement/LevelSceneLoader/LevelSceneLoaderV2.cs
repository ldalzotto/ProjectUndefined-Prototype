using UnityEngine;
using System.Collections;
using UnityEditor;
using CoreGame;
using System.Collections.Generic;
using System.Linq;
using LevelManagement;
using UnityEditor.SceneManagement;

namespace Editor_LevelSceneLoader
{
    public class LevelSceneLoaderV2 : EditorWindow
    {
        [MenuItem("Level/SceneLoadV2")]
        static void Init()
        {
            LevelSceneLoaderV2 window = (LevelSceneLoaderV2) EditorWindow.GetWindow(typeof(LevelSceneLoaderV2));
            window.Show();
        }

        #region Styles

        private GUIStyle leftAlignedText;

        #endregion

        private LevelManager levelManager;
        private LevelHierarchyConfiguration levelHierarchyConfiguration;
        private ChunkZonesSceneConfiguration chunkZonesConfiguration;
        private Dictionary<SceneAsset, bool> sceneLoadElligibility = new Dictionary<SceneAsset, bool>();

        private void InitStyles()
        {
            if (this.leftAlignedText == null)
            {
                this.leftAlignedText = new GUIStyle(EditorStyles.label);
                this.leftAlignedText.alignment = TextAnchor.MiddleLeft;
            }
        }

        private void OnGUI()
        {
            this.InitStyles();
            if (GUILayout.Button("Refresh scenes to load."))
            {
                var scenesToLoad = this.RefreshScenesToLoad();
                if (scenesToLoad != null)
                {
                    this.sceneLoadElligibility.Clear();
                    foreach (var sceneToLoad in scenesToLoad)
                    {
                        this.sceneLoadElligibility[sceneToLoad] = true;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
            if (GUILayout.Button(new GUIContent("*", "Select all"), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                foreach (var sceneToLoad in this.sceneLoadElligibility.Keys.ToList())
                {
                    this.sceneLoadElligibility[sceneToLoad] = true;
                }
            }

            if (GUILayout.Button(new GUIContent("o", "Unselect all"), EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                foreach (var sceneToLoad in this.sceneLoadElligibility.Keys.ToList())
                {
                    this.sceneLoadElligibility[sceneToLoad] = false;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.textArea);


            foreach (var sceneToLoad in this.sceneLoadElligibility.Keys.ToList())
            {
                EditorGUILayout.BeginHorizontal();
                this.sceneLoadElligibility[sceneToLoad] = EditorGUILayout.Toggle(this.sceneLoadElligibility[sceneToLoad]);
                EditorGUILayout.LabelField(sceneToLoad.name, this.leftAlignedText);
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("PROCESS"))
            {
                foreach (var sceneToLoad in this.sceneLoadElligibility)
                {
                    if (sceneToLoad.Value)
                    {
                        this.SceneLoadWithoutDuplicate(sceneToLoad.Key.name);
                    }
                    else
                    {
                        this.SceneUnLoad(sceneToLoad.Key.name);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        private List<SceneAsset> RefreshScenesToLoad()
        {
            this.levelManager = GameObject.FindObjectOfType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogError("No level manager found.");
                return null;
            }
            else
            {
                this.levelHierarchyConfiguration = AssetFinder.SafeSingleAssetFind<LevelHierarchyConfiguration>("t:" + typeof(LevelHierarchyConfiguration).Name);
                this.chunkZonesConfiguration = AssetFinder.SafeSingleAssetFind<ChunkZonesSceneConfiguration>("t:" + typeof(ChunkZonesSceneConfiguration).Name);
                if (chunkZonesConfiguration == null)
                {
                    Debug.LogError("The chunk zone configuration has not been found.");
                    return null;
                }
            }

            return levelHierarchyConfiguration.GetLevelHierarchy(levelManager.GetCurrentLevel()).ConvertAll((chunkId) => { return chunkZonesConfiguration.ConfigurationInherentData[chunkId].scene; });
        }

        private void SceneLoadWithoutDuplicate(string sceneToLoadName)
        {
            var sceneNB = EditorSceneManager.sceneCount;
            bool load = true;
            for (var i = 0; i < sceneNB; i++)
            {
                if (EditorSceneManager.GetSceneAt(i).name == sceneToLoadName)
                {
                    load = false;
                }
            }

            if (load)
            {
                var scene = AssetFinder.SafeSingleAssetFind<SceneAsset>(sceneToLoadName + " t:Scene");
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Additive);
            }
        }

        private void SceneUnLoad(string sceneToUnloadName)
        {
            var sceneNB = EditorSceneManager.sceneCount;
            bool unload = false;
            int sceneIndex = 0;
            for (var i = 0; i < sceneNB; i++)
            {
                if (EditorSceneManager.GetSceneAt(i).name == sceneToUnloadName)
                {
                    unload = true;
                    sceneIndex = i;
                }
            }

            if (unload)
            {
                EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(sceneIndex), true);
            }
        }
    }
}