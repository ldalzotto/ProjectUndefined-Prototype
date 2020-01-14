using CoreGame;
using InteractiveObjectAction;
using PlayerAim;
using PlayerObject;
using UnityEngine;

namespace InputDynamicTextMenu
{
    public class InputDynamicTextMenuManager : GameSingleton<InputDynamicTextMenuManager>
    {
        private GameObject UIInputDynamicTextMenuModulesContainer;

        private LocomotionTextModule LocomotionTextModule;
        private CameraTextModule CameraTextModule;
        private TimeStopModule TimeStopModule;
        private FiringModeEnterTextModule FiringModeEnterTextModule;
        private OnTargettingTextModule OnTargettingTextModule;

        public InputDynamicTextMenuManager()
        {
            var InputDynamicTextMenuConfiguration = InputDynamicTextMenuConfigurationGameObject.Get().InputDynamicTextMenuConfiguration;
            this.UIInputDynamicTextMenuModulesContainer = MonoBehaviour.Instantiate(InputDynamicTextMenuConfiguration.UIInputDynamicTextMenuModulesContainerPrefab, CoreGameSingletonInstances.GameCanvas.transform);
            var containerRectTransform = this.UIInputDynamicTextMenuModulesContainer.GetComponent<RectTransform>();

            this.RegisterEvents();

            this.LocomotionTextModule = new LocomotionTextModule(containerRectTransform);
            this.CameraTextModule = new CameraTextModule(containerRectTransform);
            this.TimeStopModule = new TimeStopModule(containerRectTransform);
            this.FiringModeEnterTextModule = new FiringModeEnterTextModule(containerRectTransform);
            this.OnTargettingTextModule = new OnTargettingTextModule(containerRectTransform);
            this.OnTargettingTextModule.Disable();
        }

        private void RegisterEvents()
        {
            var PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            if (PlayerInteractiveObject is IEM_IPlayerAimingFiringRegisteringEventsExposedMethod IPlayerFiringRegisteringEventsExposedMethod)
            {
                IPlayerFiringRegisteringEventsExposedMethod.RegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
                IPlayerFiringRegisteringEventsExposedMethod.RegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            }
        }

        private void UnRegisterEvents()
        {
            var PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            if (PlayerInteractiveObject is IEM_IPlayerAimingFiringRegisteringEventsExposedMethod IPlayerFiringRegisteringEventsExposedMethod)
            {
                IPlayerFiringRegisteringEventsExposedMethod.UnRegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
                IPlayerFiringRegisteringEventsExposedMethod.UnRegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            }
        }

        public override void OnDestroy()
        {
            GameObject.Destroy(this.UIInputDynamicTextMenuModulesContainer);
            this.UnRegisterEvents();
            base.OnDestroy();
        }


        private void OnPlayerStartTargetting(InteractiveObjectActionInherentData interactiveObjectActionInherentData)
        {
            this.OnTargettingTextModule.Enable();
        }

        private void OnPlayerStoppedTargetting(InteractiveObjectActionInherentData interactiveObjectActionInherentData)
        {
            this.OnTargettingTextModule.Disable();
        }
    }
}