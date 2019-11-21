using System.IO;
using CoreGame;
using Input;
using LevelManagement;
using UnityEngine;
using UnityEngine.UI;

namespace StartMenu
{
    public class StartMenuManager : GameSingleton<StartMenuManager>
    {
        #region External Dependencies

        private StartLevelManager StartLevelManager = StartLevelManager.Get();
        private GameInputManager GameInputManager = GameInputManager.Get();
        #endregion

        private StartMenuGameObject StartMenuGameObject;

        public void Init()
        {
            /// Events
            ControlsWindowExitedEvent.Get().RegisterControlsWindowExitedEventAction(this.OnControlWindowExited);
            ;
            var StartMenuCanvas = CoreGameSingletonInstances.GameCanvas;
            var StartMenuPrefabConfiguration = StartMenuSingletonInstances.StartMenuStaticConfigurationManager.StartMenuStaticConfiguration.StartMenuPrefabConfiguration;
            this.StartMenuGameObject = new StartMenuGameObject(StartMenuCanvas, StartMenuPrefabConfiguration);

            IGameProgressionStateManagerDataRetriever IGameProgressionStateManagerDataRetriever = StartMenuSingletonInstances.GameProgressionStateManager;

            var NewGameButton = this.StartMenuGameObject.CreateStartMenuButton();
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

            /*
            var ContinueButton = MonoBehaviour.Instantiate(StartMenuPrefabConfiguration.StartMenuButtonBasePrefab, StartMenuCanvas.transform);
            ContinueButton.GetComponentInChildren<Text>().text = "Continue";
            ((RectTransform) ContinueButton.transform).anchoredPosition = new Vector2(0, -40);
            ContinueButton.interactable = IGameProgressionStateManagerDataRetriever.HasAlreadyPlayed();
            if (ContinueButton.IsInteractable())
            {
                ContinueButton.onClick.AddListener(() => { LevelTransitionManager.Get().OnStartMenuToLevel(this.StartLevelManager.GetStartLevelID()); });
            }
            */

            var ControlsButton = this.StartMenuGameObject.CreateStartMenuButton();
            ControlsButton.GetComponentInChildren<Text>().text = "Controls";
            ((RectTransform) ControlsButton.transform).anchoredPosition = new Vector2(0, -40);
            ControlsButton.onClick.AddListener(this.OnControlsButtonClicked);
        }

        public void Tick(float d)
        {
            if (GameInputManager.CurrentInput.MenuExitD())
            {
                StartMenuEscapePressedEvent.Get().OnStartMenuEscapePressed();
            }
        }

        private void OnControlsButtonClicked()
        {
            this.StartMenuGameObject.Hide();
            ControlsWindowManager.Get().Show();
        }

        private void OnControlWindowExited()
        {
            this.StartMenuGameObject.Show();
        }
    }

    public class StartMenuGameObject
    {
        private GameObject StartMenuParent;
        private StartMenuPrefabConfiguration StartMenuPrefabConfiguration;

        public StartMenuGameObject(Canvas parentCanvas, StartMenuPrefabConfiguration StartMenuPrefabConfiguration)
        {
            this.StartMenuPrefabConfiguration = StartMenuPrefabConfiguration;
            this.StartMenuParent = new GameObject("StartMenuGameObject", typeof(RectTransform));
            this.StartMenuParent.transform.parent = parentCanvas.transform;
            var StartMenuParentRectTransform = (this.StartMenuParent.transform as RectTransform);
            StartMenuParentRectTransform.Reset(RectTransformSetup.BOTTOM_LEFT);
        }

        public Button CreateStartMenuButton()
        {
            return MonoBehaviour.Instantiate(this.StartMenuPrefabConfiguration.StartMenuButtonBasePrefab, this.StartMenuParent.transform);
        }

        public void Hide()
        {
            this.StartMenuParent.SetActive(false);
        }

        public void Show()
        {
            this.StartMenuParent.SetActive(true);
        }
    }
}