using CoreGame;
using UnityEngine;

namespace InputDynamicTextMenu
{
    public class InputDynamicTextMenuManager : GameSingleton<InputDynamicTextMenuManager>
    {
        private GameObject UIInputDynamicTextMenuModulesContainer;

        private LocomotionTextModule LocomotionTextModule;
        private CameraTextModule CameraTextModule;
        private FiringModeEnterTextModule FiringModeEnterTextModule;

        public InputDynamicTextMenuManager()
        {
            var InputDynamicTextMenuConfiguration = InputDynamicTextMenuConfigurationGameObject.Get().InputDynamicTextMenuConfiguration;
            this.UIInputDynamicTextMenuModulesContainer = MonoBehaviour.Instantiate(InputDynamicTextMenuConfiguration.UIInputDynamicTextMenuModulesContainerPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            var containerRectTransform = this.UIInputDynamicTextMenuModulesContainer.GetComponent<RectTransform>();

            this.LocomotionTextModule = new LocomotionTextModule(containerRectTransform);
            this.CameraTextModule = new CameraTextModule(containerRectTransform);
            this.FiringModeEnterTextModule = new FiringModeEnterTextModule(containerRectTransform);
        }

        public override void OnDestroy()
        {
            GameObject.Destroy(this.UIInputDynamicTextMenuModulesContainer);
            base.OnDestroy();
        }
    }
}