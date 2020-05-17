using Editor_GenericConfigurationCreation;
using System;
using UnityEditor;

namespace Editor_MainGameCreationWizard
{
    public class GameCreationWizard : EditorWindow
    {
        [MenuItem("CreationWizard/GameCreationWizard")]
        public static void Init()
        {
            var window = EditorWindow.GetWindow<GameCreationWizard>();
            window.Show();
        }

        public static GameCreationWizard InitWithSelected(string key)
        {
            var window = EditorWindow.GetWindow<GameCreationWizard>();
            window.Show();
            if (window.playerActionCreationWizardEditorProfile == null)
            {
                window.playerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<GameCreationWizardEditorProfile>("t:" + typeof(GameCreationWizardEditorProfile).ToString());
                if (window.playerActionCreationWizardEditorProfile != null)
                {
                    window.playerActionCreationWizardEditorProfile.Init(() => { window.Repaint(); });
                    window.playerActionCreationWizardEditorProfile.SetSelectedKey(key);
                }
            }
            return window;
        }

        public static void InitGenericCreator(Type configurationType)
        {
            var GameCreationWizard = InitWithSelected(typeof(GenericConfigurationCreationWizard).Name);
            var GenericConfigurationCreationWizard = GameCreationWizard.playerActionCreationWizardEditorProfile.GetSelectedConf() as GenericConfigurationCreationWizard;
            GenericConfigurationCreationWizard.SetSelectedConfiguration(configurationType);
        }

        private GameCreationWizardEditorProfile playerActionCreationWizardEditorProfile;

        private void OnGUI()
        {
            this.playerActionCreationWizardEditorProfile = EditorGUILayout.ObjectField(this.playerActionCreationWizardEditorProfile, typeof(GameCreationWizardEditorProfile), false) as GameCreationWizardEditorProfile;
            if (this.playerActionCreationWizardEditorProfile == null)
            {
                this.playerActionCreationWizardEditorProfile = AssetFinder.SafeSingleAssetFind<GameCreationWizardEditorProfile>("t:" + typeof(GameCreationWizardEditorProfile).ToString());
            }
            if (this.playerActionCreationWizardEditorProfile != null)
            {
                this.playerActionCreationWizardEditorProfile.GUITick(() => { Repaint(); });
                ICreationWizardEditor selectedTab = this.playerActionCreationWizardEditorProfile.GetSelectedConf();
                if (selectedTab != null)
                {
                    selectedTab.OnGUI();
                }
            }
        }

    }


}