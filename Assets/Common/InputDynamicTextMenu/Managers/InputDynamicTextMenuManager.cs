using AnimatorPlayable;
using CoreGame;
using Firing;
using PlayerActions;
using PlayerObject;
using ProjectileDeflection;
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
        private DelflectionTextModule DelflectionTextModule;

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
            this.DelflectionTextModule = new DelflectionTextModule(containerRectTransform);
            DelflectionTextModule.Disable();
        }

        private void RegisterEvents()
        {
            var PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            if (PlayerInteractiveObject is IEM_IPlayerFiringRegisteringEventsExposedMethod IPlayerFiringRegisteringEventsExposedMethod)
            {
                IPlayerFiringRegisteringEventsExposedMethod.RegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
                IPlayerFiringRegisteringEventsExposedMethod.RegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            }

            if (PlayerInteractiveObject is IEM_PlayerLowHealthInteractiveObjectExposedMethods PlayerLowHealthInteractiveObjectExposedMethods)
            {
                PlayerLowHealthInteractiveObjectExposedMethods.RegisterPlayerLowHealthStartedEvent(this.OnPlayerLowHealthStarted);
                PlayerLowHealthInteractiveObjectExposedMethods.RegisterPlayerLowHealthEndedEvent(this.OnPlayerLowHealthEnded);
            }
        }

        private void UnRegisterEvents()
        {
            var PlayerInteractiveObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;

            if (PlayerInteractiveObject is IEM_IPlayerFiringRegisteringEventsExposedMethod IPlayerFiringRegisteringEventsExposedMethod)
            {
                IPlayerFiringRegisteringEventsExposedMethod.UnRegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
                IPlayerFiringRegisteringEventsExposedMethod.UnRegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            }

            if (PlayerInteractiveObject is IEM_PlayerLowHealthInteractiveObjectExposedMethods PlayerLowHealthInteractiveObjectExposedMethods)
            {
                PlayerLowHealthInteractiveObjectExposedMethods.UnRegisterPlayerLowHealthStartedEvent(this.OnPlayerLowHealthStarted);
                PlayerLowHealthInteractiveObjectExposedMethods.UnRegisterPlayerLowHealthEndedEvent(this.OnPlayerLowHealthEnded);
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

        private void OnPlayerLowHealthStarted()
        {
            this.DelflectionTextModule.Enable();
        }

        private void OnPlayerLowHealthEnded()
        {
            this.DelflectionTextModule.Disable();
        }
    }
}