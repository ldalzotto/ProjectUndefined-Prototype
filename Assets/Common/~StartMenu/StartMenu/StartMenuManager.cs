using System.IO;
using CoreGame;
using LevelManagement;
using UnityEngine;
using UnityEngine.UI;

namespace StartMenu
{
    public class StartMenuManager
    {
        public static void Init()
        {
            var startLevelManager = StartLevelManager.Get();
            var StartMenuPrefabConfiguration = StartMenuSingletonInstances.StartMenuStaticConfigurationManager.StartMenuStaticConfiguration.StartMenuPrefabConfiguration;
            var StartMenuCanvas = CoreGameSingletonInstances.GameCanvas;

            IGameProgressionStateManagerDataRetriever IGameProgressionStateManagerDataRetriever = StartMenuSingletonInstances.GameProgressionStateManager;

            var NewGameButton = MonoBehaviour.Instantiate(StartMenuPrefabConfiguration.StartMenuButtonBasePrefab, StartMenuCanvas.transform);
            NewGameButton.GetComponentInChildren<Text>().text = "New Game";
            ((RectTransform) NewGameButton.transform).anchoredPosition = new Vector2(0, 40);

            NewGameButton.onClick.AddListener(() =>
            {
                //Destroy all saved data
                var persistanceDirectory = new DirectoryInfo(Application.persistentDataPath);
                foreach (var directory in persistanceDirectory.GetDirectories())
                {
                    directory.Delete(true);
                }

                LevelTransitionManager.Get().OnStartMenuToLevel(LevelManagementConfigurationGameObject.Get().GlobalLevelConfiguration.NewGameStartLevelID);
            });

            var ContinueButton = MonoBehaviour.Instantiate(StartMenuPrefabConfiguration.StartMenuButtonBasePrefab, StartMenuCanvas.transform);
            ContinueButton.GetComponentInChildren<Text>().text = "Continue";
            ((RectTransform) ContinueButton.transform).anchoredPosition = new Vector2(0, -40);
            ContinueButton.interactable = IGameProgressionStateManagerDataRetriever.HasAlreadyPlayed();
            if (ContinueButton.IsInteractable())
            {
                ContinueButton.onClick.AddListener(() => { LevelTransitionManager.Get().OnStartMenuToLevel(startLevelManager.GetStartLevelID()); });
            }
        }
    }
}