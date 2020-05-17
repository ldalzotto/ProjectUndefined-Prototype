using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using CoreGame;
using ConfigurationEditor;
using System;

public class GameConfigurationRefresher : EditorWindow
{
    [MenuItem("Configuration/GameConfigurationRefresher")]
    static void Init()
    {
        GameConfigurationRefresher window = (GameConfigurationRefresher)EditorWindow.GetWindow(typeof(GameConfigurationRefresher));
        window.Show();
    }

    private GameConfigurationRefresherProfile GameConfigurationRefresherProfile;
    private List<SingleGameConfiguration> foundGameConfigurations = new List<SingleGameConfiguration>();

    private void OnGUI()
    {
        GameConfigurationRefresherProfile = (GameConfigurationRefresherProfile)EditorGUILayout.ObjectField(GameConfigurationRefresherProfile, typeof(GameConfigurationRefresherProfile), false);
        if (GameConfigurationRefresherProfile == null)
        {
            GameConfigurationRefresherProfile = AssetFinder.SafeSingleAssetFind<GameConfigurationRefresherProfile>("t:" + typeof(GameConfigurationRefresherProfile).Name);
        }

        if (GameConfigurationRefresherProfile != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Found game configurations : ");
            if (foundGameConfigurations.Count == 0)
            {
                var childGameConfigurationTypes = TypeHelper.GetAllTypeAssignableFrom(typeof(GameConfiguration));
                if (childGameConfigurationTypes != null)
                {
                    foreach (var gameConfigurationType in childGameConfigurationTypes)
                    {
                        foundGameConfigurations.Add(new SingleGameConfiguration((GameConfiguration)AssetFinder.SafeSingleAssetFind<UnityEngine.Object>("t:" + gameConfigurationType.Name),
                            GameConfigurationRefresherProfile));
                    }
                }
            }

            if (GUILayout.Button("REFRESH ALL"))
            {
                foreach (var gameConfiguration in foundGameConfigurations)
                {
                    gameConfiguration.Refresh();
                }
            }

            foreach (var gameConfiguration in foundGameConfigurations)
            {
                EditorGUILayout.BeginVertical();
                gameConfiguration.OnGui();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Separator();
            }


        }
    }

    class SingleGameConfiguration
    {
        private GameConfiguration gameConfiguration;
        private GameConfigurationRefresherProfile GameConfigurationRefresherProfile;

        public SingleGameConfiguration(GameConfiguration gameConfiguration, GameConfigurationRefresherProfile GameConfigurationRefresherProfile)
        {
            this.gameConfiguration = gameConfiguration;
            this.GameConfigurationRefresherProfile = GameConfigurationRefresherProfile;
        }

        private SingleGameConfigurationRefresherProfile SingleGameConfigurationRefresherProfile;

        public void OnGui()
        {
            if (!GameConfigurationRefresherProfile.GameConfgurationsRefreshEditorProfile.ContainsKey(gameConfiguration.GetType().Name))
            {
                GameConfigurationRefresherProfile.GameConfgurationsRefreshEditorProfile.Add(gameConfiguration.GetType().Name, new SingleGameConfigurationRefresherProfile());
            }
            else
            {
                SingleGameConfigurationRefresherProfile = GameConfigurationRefresherProfile.GameConfgurationsRefreshEditorProfile[gameConfiguration.GetType().Name];
            }

            SingleGameConfigurationRefresherProfile.Folded = EditorGUILayout.Foldout(SingleGameConfigurationRefresherProfile.Folded, gameConfiguration.GetType().Name, true);
            if (SingleGameConfigurationRefresherProfile.Folded)
            {
                //ConfigurationSerialization
                if (GUILayout.Button(new GUIContent("R", "Refresh"), EditorStyles.miniButton))
                {
                    this.Refresh();
                }
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(this.gameConfiguration, typeof(GameConfiguration), false);
                EditorGUILayout.Separator();
                DynamicEditorCreation.Get().CreateEditor(this.gameConfiguration).OnInspectorGUI();
                EditorGUI.EndDisabledGroup();
            }
        }

        public void Refresh()
        {
            var fields = this.gameConfiguration.GetType().GetFields();
            foreach (var field in fields)
            {
                field.SetValue(this.gameConfiguration, AssetFinder.SafeSingleAssetFind<UnityEngine.Object>("t:" + field.FieldType.Name));
            }
            EditorUtility.SetDirty(this.gameConfiguration);
        }
    }
}
