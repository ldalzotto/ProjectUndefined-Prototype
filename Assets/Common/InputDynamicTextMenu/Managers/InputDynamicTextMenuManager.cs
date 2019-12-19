using AnimatorPlayable;
using CoreGame;
using PlayerLowHealth_Interfaces;
using PlayerObject_Interfaces;
using UnityEngine;

namespace InputDynamicTextMenu
{
    public class InputDynamicTextMenuManager : GameSingleton<InputDynamicTextMenuManager>
    {
        private GameObject UIInputDynamicTextMenuModulesContainer;

        private LocomotionTextModule LocomotionTextModule;
        private CameraTextModule CameraTextModule;
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
            this.FiringModeEnterTextModule = new FiringModeEnterTextModule(containerRectTransform);
            this.OnTargettingTextModule = new OnTargettingTextModule(containerRectTransform);
            this.OnTargettingTextModule.Disable();
            this.DelflectionTextModule = new DelflectionTextModule(containerRectTransform);
            DelflectionTextModule.Disable();
        }

        private void RegisterEvents()
        {
            PlayerStartTargettingEvent.Get().RegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
            PlayerStoppedTargettingEvent.Get().RegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            
            PlayerLowHealthStartedEvent.Get().RegisterPlayerLowHealthStartedEvent(this.OnPlayerLowHealthStarted);
            PlayerLowHealthEndedEvent.Get().RegisterPlayerLowHealthEndedEvent(this.OnPlayerLowHealthEnded);
        }

        private void UnRegisterEvents()
        {
            PlayerStartTargettingEvent.Get().UnRegisterOnPlayerStartTargettingEvent(this.OnPlayerStartTargetting);
            PlayerStoppedTargettingEvent.Get().UnRegisterOnPlayerStoppedTargettingEvent(this.OnPlayerStoppedTargetting);
            
            PlayerLowHealthStartedEvent.Get().UnRegisterPlayerLowHealthStartedEvent(this.OnPlayerLowHealthStarted);
            PlayerLowHealthEndedEvent.Get().UnRegisterPlayerLowHealthEndedEvent(this.OnPlayerLowHealthEnded);
        }

        public override void OnDestroy()
        {
            GameObject.Destroy(this.UIInputDynamicTextMenuModulesContainer);
            this.UnRegisterEvents();
            base.OnDestroy();
        }


        private void OnPlayerStartTargetting(A_AnimationPlayableDefinition anim)
        {
            this.OnTargettingTextModule.Enable();
        }

        private void OnPlayerStoppedTargetting()
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