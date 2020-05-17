using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    public class GameDesignerEditor : EditorWindow
    {
        public const string GameDesignerProfilePath = "Assets/Editor/GameDesigner";

        [MenuItem("GameDesigner/GameDesignerEditor")]
        static void Init()
        {
            var instanceIndex = GameDesignerWindowManager.GetGameDesignerEditorInstance();
            GameDesignerEditor window = GameDesignerWindowManager.gameDesignerEditors[instanceIndex];
            window.InitEditorData(instanceIndex);
            window.ChoiceTree.Init(() => { window.Repaint(); });
            window.Show();
        }

        public static GameDesignerEditor InitWithSelectedKey(Type designerModuleType)
        {
            var instanceIndex = GameDesignerWindowManager.GetGameDesignerEditorInstance();
            GameDesignerEditor window = GameDesignerWindowManager.gameDesignerEditors[instanceIndex];
            window.InitEditorData(instanceIndex);
            window.ChoiceTree.Init(() => { window.Repaint(); });
            window.ChoiceTree.SetSelectedKey(designerModuleType);
            window.Show();
            return window;
        }

        private GameDesignerEditorProfile GameDesignerEditorProfile;

        public GameDesignerChoiceTree ChoiceTree;

        public IGameDesignerModule GetCrrentGameDesignerModule()
        {
            return this.GameDesignerEditorProfile.CurrentGameDesignerModule;
        }

        public void InitEditorData(int instanceIndex)
        {
            if (this.GameDesignerEditorProfile == null)
            {
                var GameDesignerEditorProfiles = AssetFinder.SafeAssetFind<GameDesignerEditorProfile>("t:" + typeof(GameDesignerEditorProfile).Name);
                foreach (var GameDesignerEditorProfile in GameDesignerEditorProfiles)
                {
                    if (GameDesignerEditorProfile.GameDesignerProfileInstanceIndex == instanceIndex)
                    {
                        this.GameDesignerEditorProfile = GameDesignerEditorProfile;
                        break;
                    }
                }

                if (this.GameDesignerEditorProfile == null)
                {
                    var CreatedGameDesignerEditorProfile = (GameDesignerEditorProfile)GameDesignerEditorProfile.CreateInstance(typeof(GameDesignerEditorProfile));
                    CreatedGameDesignerEditorProfile.GameDesignerProfileInstanceIndex = instanceIndex;
                    AssetDatabase.CreateAsset(CreatedGameDesignerEditorProfile, GameDesignerProfilePath + "/GameDesignerEditorProfile_" + instanceIndex + ".asset");
                    this.GameDesignerEditorProfile = CreatedGameDesignerEditorProfile;
                }
            }
            if (this.GameDesignerEditorProfile != null)
            {

                if (this.ChoiceTree == null)
                {
                    this.ChoiceTree = new GameDesignerChoiceTree(this.GameDesignerEditorProfile);
                }
            }
        }

        private void OnGUI()
        {
            if (this.GameDesignerEditorProfile != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(GUILayout.Width(200f));

                if (this.ChoiceTree == null)
                {
                    this.ChoiceTree = new GameDesignerChoiceTree(this.GameDesignerEditorProfile);
                }
                if (this.ChoiceTree != null)
                {
                    this.ChoiceTree.GUITick(() => { this.Repaint(); });
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (this.GameDesignerEditorProfile.CurrentGameDesignerModule != null)
                {
                    this.GameDesignerEditorProfile.ScrollPosition = EditorGUILayout.BeginScrollView(this.GameDesignerEditorProfile.ScrollPosition);
                    this.GameDesignerEditorProfile.CurrentGameDesignerModule.GUITick(ref this.GameDesignerEditorProfile);
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.ObjectField(this.GameDesignerEditorProfile, typeof(UnityEngine.Object), false);

            if (GUI.changed) { this.Repaint(); }
        }

        private void OnSelectionChange()
        {
            this.Repaint();
        }
    }


    public static class GameDesignerWindowManager
    {

        public static Dictionary<int, GameDesignerEditor> gameDesignerEditors;

        public static int GetGameDesignerEditorInstance()
        {
            if (gameDesignerEditors == null)
            {
                gameDesignerEditors = new Dictionary<int, GameDesignerEditor>();

                var GameDesignerEditorProfiles = AssetFinder.SafeAssetFind<GameDesignerEditorProfile>("t:" + typeof(GameDesignerEditorProfile).Name);
                foreach (var GameDesignerEditorProfile in GameDesignerEditorProfiles)
                {
                    gameDesignerEditors[GameDesignerEditorProfile.GameDesignerProfileInstanceIndex] = null;
                }
            }

            int pickedIndex = 0;
            if (gameDesignerEditors.Count > 0) 
            {
                var maxIndex = gameDesignerEditors.Keys.ToList().Max();
                bool hasFoundPick = false;
                for (var i = 0; i < maxIndex; i++)
                {
                    if (!gameDesignerEditors.ContainsKey(i) || gameDesignerEditors[i] == null)
                    {
                        pickedIndex = i;
                        hasFoundPick = true;
                        break;
                    }
                }

                if (!hasFoundPick)
                {
                    pickedIndex = maxIndex + 1;
                }
            }

            gameDesignerEditors[pickedIndex] = (GameDesignerEditor)EditorWindow.CreateInstance(typeof(GameDesignerEditor));
            return pickedIndex;
        }

    }
}
