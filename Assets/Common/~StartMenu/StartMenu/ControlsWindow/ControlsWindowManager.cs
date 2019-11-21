using CoreGame;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StartMenu
{
    public class ControlsWindowManager : GameSingleton<ControlsWindowManager>
    {
        private ControlsWindowGameObject ControlsWindowGameObject;

        public ControlsWindowManager()
        {
            this.ControlsWindowGameObject = new ControlsWindowGameObject(CoreGameSingletonInstances.GameCanvas);
          
            this.ControlsWindowGameObject.Hide();
        }

        public void Destroy()
        {
            this.ControlsWindowGameObject.Destroy();
        }

        /// <summary>
        /// It registers to <see cref="StartMenuEscapePressedEvent"/> to exit the window <see cref="Exit"/>
        /// </summary>
        public void Show()
        {
            StartMenuEscapePressedEvent.Get().RegisterOnStartMenuEscapePressedEventAction(this.Exit);
            this.ControlsWindowGameObject.Show();
        }

        private void Exit()
        {
            StartMenuEscapePressedEvent.Get().UnRegisterOnStartMenuEscapePressedEventAction(this.Exit);
            this.ControlsWindowGameObject.Hide();
            ControlsWindowExitedEvent.Get().OnControlsWindowExited();
        }
    }

    class ControlsWindowGameObject
    {
        private GameObject controlsWindowGameObjectParent;

        public ControlsWindowGameObject(Canvas parentCanvas)
        {
            this.controlsWindowGameObjectParent = MonoBehaviour.Instantiate(StartMenuSingletonInstances.StartMenuStaticConfigurationManager.StartMenuStaticConfiguration.StartMenuPrefabConfiguration.ControlsWindowPrefab);

            foreach (var InputKeyIconInitializer in this.controlsWindowGameObjectParent.GetComponentsInChildren<InputIconInitializer>())
            {
                InputKeyIconInitializer.Build();
            }

            this.controlsWindowGameObjectParent.transform.parent = parentCanvas.transform;
            (this.controlsWindowGameObjectParent.transform as RectTransform).Reset(RectTransformSetup.CENTER);
            (this.controlsWindowGameObjectParent.transform as RectTransform).pivot = new Vector2(0.5f,0.5f);
            (this.controlsWindowGameObjectParent.transform as RectTransform).localPosition = Vector3.zero;
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.controlsWindowGameObjectParent.transform as RectTransform);
        }

        public void Destroy()
        {
            MonoBehaviour.Destroy(this.controlsWindowGameObjectParent);
        }

        public void Show()
        {
            this.controlsWindowGameObjectParent.SetActive(true);
        }

        public void Hide()
        {
            this.controlsWindowGameObjectParent.SetActive(false);
        }
    }
}