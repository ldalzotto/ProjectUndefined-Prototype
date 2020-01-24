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

            var Level1Button = this.StartMenuGameObject.CreateStartMenuButton();
            Level1Button.GetComponentInChildren<Text>().text = "Level 1";

            (Level1Button.transform as RectTransform).Reset(RectTransformSetup.CENTER);
            (Level1Button.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
            (Level1Button.transform as RectTransform).localPosition = Vector3.zero;
            (Level1Button.transform as RectTransform).sizeDelta = new Vector2(200, 30);
            (Level1Button.transform as RectTransform).anchoredPosition = new Vector2(0, 40);

            Level1Button.onClick.AddListener(() =>
            {
                //Destroy all saved data
                var persistanceDirectory = new DirectoryInfo(Application.persistentDataPath);
                foreach (var directory in persistanceDirectory.GetDirectories())
                {
                    directory.Delete(true);
                }

                LevelTransitionManager.Get().OnStartMenuToLevel(LevelManagementConfigurationGameObject.Get().GlobalLevelConfiguration.NewGameStartLevelID);
            });

            var InfiniteLevelButton = this.StartMenuGameObject.CreateStartMenuButton();
            InfiniteLevelButton.GetComponentInChildren<Text>().text = "Infinite Level";

            (InfiniteLevelButton.transform as RectTransform).Reset(RectTransformSetup.CENTER);
            (InfiniteLevelButton.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
            (InfiniteLevelButton.transform as RectTransform).localPosition = Vector3.zero;
            (InfiniteLevelButton.transform as RectTransform).sizeDelta = new Vector2(200, 30);
            (InfiniteLevelButton.transform as RectTransform).anchoredPosition = new Vector2(0, -40);

            InfiniteLevelButton.onClick.AddListener(() =>
            {
                //Destroy all saved data
                var persistanceDirectory = new DirectoryInfo(Application.persistentDataPath);
                foreach (var directory in persistanceDirectory.GetDirectories())
                {
                    directory.Delete(true);
                }

                LevelTransitionManager.Get().OnStartMenuToLevel(LevelZonesID.INFINITE_LEVEL);
            });

            var ControlsButton = this.StartMenuGameObject.CreateStartMenuButton();
            ControlsButton.GetComponentInChildren<Text>().text = "Controls";


            (ControlsButton.transform as RectTransform).Reset(RectTransformSetup.CENTER);
            (ControlsButton.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
            (ControlsButton.transform as RectTransform).localPosition = Vector3.zero;
            (ControlsButton.transform as RectTransform).sizeDelta = new Vector2(200, 30);
            (ControlsButton.transform as RectTransform).anchoredPosition = new Vector2(0, -120);

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
            StartMenuParentRectTransform.Reset(RectTransformSetup.CENTER);
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