using System;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Input
{
    public class GameInputManager : GameSingleton<GameInputManager>
    {
        private GameInputSystemUpdater GameInputSystemUpdater;

        protected XInput currentInput;

        public XInput CurrentInput
        {
            get => currentInput;
        }

        public virtual void Init(CursorLockMode CursorLockMode)
        {
            this.GameInputSystemUpdater = new GameInputSystemUpdater();
            currentInput = new GameInput(new GameInputV2(InputConfigurationGameObject.Get().InputConfiguration), InputConfigurationGameObject.Get().CoreInputConfiguration);
            Cursor.lockState = CursorLockMode;
        }

        public virtual void FixedTick()
        {
            this.GameInputSystemUpdater.FixedTick();
        }

        public virtual void Tick()
        {
            this.GameInputSystemUpdater.Tick();
        }

        public virtual void LateTick()
        {
            this.GameInputSystemUpdater.LateTick();
        }

        private class GameInput : XInput
        {
            private GameInputV2 gameInputV2;
            private CoreInputConfiguration CoreInputConfiguration;

            internal GameInputV2 GameInputV2
            {
                get => gameInputV2;
            }

            public GameInput(GameInputV2 gameInputV2, CoreInputConfiguration CoreInputConfiguration)
            {
                this.gameInputV2 = gameInputV2;
                this.CoreInputConfiguration = CoreInputConfiguration;
            }

            public bool ActionButtonD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.ACTION_DOWN);
            }

            public bool CancelButtonD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.CANCEL_DOWN);
            }

            public Vector3 LocomotionAxis()
            {
                var rawDirection = new Vector3(-Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.LEFT_DOWN_HOLD))
                                               + Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.RIGHT_DOWN_HOLD)),
                    0,
                    -Convert.ToInt32(Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.DOWN_DOWN_HOLD)))
                    + Convert.ToInt32(Convert.ToInt32(this.gameInputV2.InputConditionsMet(InputID.UP_DOWN_HOLD))));
                if (Vector3.Distance(rawDirection, Vector3.zero) > 1)
                {
                    rawDirection = rawDirection.normalized;
                }

                return rawDirection;
            }

            public Vector3 CursorDisplacement()
            {
                if (!Application.isFocused)
                {
                    return Vector3.zero;
                }

                return new Vector3(Mouse.current.delta.x.ReadValue(), 0, Mouse.current.delta.y.ReadValue()) * Screen.width * this.CoreInputConfiguration.GetCursorMovementMouseSensitivity();
            }

            public bool RotationCameraDH()
            {
                return this.gameInputV2.InputConditionsMet(InputID.CAMERA_ROTATION_DOWN_HOLD);
            }

            public float LeftRotationCamera()
            {
                return Mathf.Max(Mouse.current.delta.x.ReadValue(), 0) * Screen.width * this.CoreInputConfiguration.GetCameraMovementMouseSensitivity();
            }

            public float RightRotationCamera()
            {
                return -Mathf.Min(Mouse.current.delta.x.ReadValue(), 0) * Screen.width * this.CoreInputConfiguration.GetCameraMovementMouseSensitivity();
            }

            public bool SwitchSelectionButtonD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.SWITCH_SELECTION_DOWN);
            }

            public float CameraZoom()
            {
                return this.gameInputV2.InputConditionsMetFloat(InputID.CAMERA_ZOOM);
            }

            public bool FiringActionDown()
            {
                return this.gameInputV2.InputConditionsMet(InputID.FIRING_ACTION_DOWN);
            }

            public bool FiringActionDownHold()
            {
                return this.gameInputV2.InputConditionsMet(InputID.FIRING_ACTION_DOWN_HOLD);
            }

            public bool FiringProjectileDH()
            {
                return this.gameInputV2.InputConditionsMet(InputID.FIRING_PROJECTILE_DOWN_HOLD);
            }

            public bool MenuExitD()
            {
                return this.gameInputV2.InputConditionsMet(InputID.MENU_EXIT_DOWN);
            }

            public bool FreezeTimeDown()
            {
                return this.gameInputV2.InputConditionsMet(InputID.FREEZE_TIME_DOWN);
            }
        }

        #region Data Retrieval

        public virtual Dictionary<Key, KeyControl> GetKeyToKeyControlLookup()
        {
            return ((GameInput) this.currentInput).GameInputV2.KeyToKeyControlLookup;
        }

        public Dictionary<MouseButton, ButtonControl> GetMouseButtonControlLookup()
        {
            return ((GameInput) this.currentInput).GameInputV2.MouseButtonControlLookup;
        }

        #endregion
    }

    /// <summary>
    /// The internal state of <see cref="InputSystem"/> must be updated only once per frame. Because multiple update would lead to possible press/release
    /// buffer loss.
    /// This is achieved by tracking the update state <see cref="InputUpdatedThisFrame"/> that is resetted in the <see cref="LateTick"/> and set to true on either <see cref="Tick"/> or <see cref="LateTick"/>. 
    /// </summary>
    public class GameInputSystemUpdater
    {
        private bool InputUpdatedThisFrame;

        public GameInputSystemUpdater()
        {
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            this.InputUpdatedThisFrame = false;
        }

        public void FixedTick()
        {
            this.TickInputSystem();
        }

        public void Tick()
        {
            this.TickInputSystem();
        }

        /// <summary>
        /// Because internal update must be done only once per frame, update is performed only if <see cref="InputUpdatedThisFrame"/> is false.
        /// </summary>
        private void TickInputSystem()
        {
            if (!this.InputUpdatedThisFrame)
            {
                InputSystem.Update();
                this.InputUpdatedThisFrame = true;
            }
        }

        public void LateTick()
        {
            this.InputUpdatedThisFrame = false;
        }
    }

    class GameInputV2
    {
        private Dictionary<Key, KeyControl> keyToKeyControlLookup;
        private Dictionary<MouseButton, ButtonControl> mouseButtonControlLookup;
        private Dictionary<MouseScroll, Vector2Control> mouseScrollControlLookup;

        private InputConfiguration InputConfiguration;

        public GameInputV2(InputConfiguration inputConfiguration)
        {
            this.keyToKeyControlLookup = new Dictionary<Key, KeyControl>();
            this.mouseButtonControlLookup = new Dictionary<MouseButton, ButtonControl>();
            this.mouseScrollControlLookup = new Dictionary<MouseScroll, Vector2Control>();

            foreach (var keyBoardKeyControl in Keyboard.current.allKeys)
            {
                this.keyToKeyControlLookup[keyBoardKeyControl.keyCode] = keyBoardKeyControl;
            }

            this.mouseButtonControlLookup[MouseButton.LEFT_BUTTON] = Mouse.current.leftButton;
            this.mouseButtonControlLookup[MouseButton.RIGHT_BUTTON] = Mouse.current.rightButton;
            this.mouseScrollControlLookup[MouseScroll.SCROLL] = Mouse.current.scroll;

            InputConfiguration = inputConfiguration;
        }

        public Dictionary<Key, KeyControl> KeyToKeyControlLookup
        {
            get => keyToKeyControlLookup;
        }

        public Dictionary<MouseButton, ButtonControl> MouseButtonControlLookup
        {
            get => mouseButtonControlLookup;
        }

        public bool InputConditionsMet(InputID inputID)
        {
            return Convert.ToBoolean(InputConditionsMetFloat(inputID));
        }

        public float InputConditionsMetFloat(InputID inputID)
        {
            float inputConditionsMet = 0f;
            if (Application.isFocused)
            {
                var inputConfigurationInherentData = this.InputConfiguration.ConfigurationInherentData[inputID];

                if (inputConfigurationInherentData.AttributedMouseScroll != MouseScroll.NONE)
                {
                    inputConditionsMet = this.mouseScrollControlLookup[inputConfigurationInherentData.AttributedMouseScroll].y.ReadValue();
                    if (inputConditionsMet != 0f)
                    {
                        return inputConditionsMet;
                    }
                }

                foreach (var attibutedKey in inputConfigurationInherentData.AttributedKeys)
                {
                    if (inputConfigurationInherentData.Down)
                    {
                        inputConditionsMet = Convert.ToSingle(this.keyToKeyControlLookup[attibutedKey].wasPressedThisFrame);
                    }
                    else if (inputConfigurationInherentData.DownHold)
                    {
                        inputConditionsMet = Convert.ToSingle(this.keyToKeyControlLookup[attibutedKey].wasPressedThisFrame || this.keyToKeyControlLookup[attibutedKey].isPressed);
                    }
                    else if (inputConfigurationInherentData.Released)
                    {
                        inputConditionsMet = Convert.ToSingle(this.keyToKeyControlLookup[attibutedKey].wasReleasedThisFrame);
                    }

                    if (inputConditionsMet != 0f)
                    {
                        return inputConditionsMet;
                    }
                }

                foreach (var attrubuteMouseButton in inputConfigurationInherentData.AttributedMouseButtons)
                {
                    if (inputConfigurationInherentData.Down)
                    {
                        inputConditionsMet = Convert.ToSingle(this.mouseButtonControlLookup[attrubuteMouseButton].wasPressedThisFrame);
                    }
                    else if (inputConfigurationInherentData.DownHold)
                    {
                        inputConditionsMet = Convert.ToSingle(this.mouseButtonControlLookup[attrubuteMouseButton].wasPressedThisFrame || this.mouseButtonControlLookup[attrubuteMouseButton].isPressed);
                    }
                    else if (inputConfigurationInherentData.Released)
                    {
                        inputConditionsMet = Convert.ToSingle(this.mouseButtonControlLookup[attrubuteMouseButton].wasReleasedThisFrame);
                    }

                    if (inputConditionsMet != 0f)
                    {
                        return inputConditionsMet;
                    }
                }
            }

            return inputConditionsMet;
        }
    }

    public interface XInput
    {
        Vector3 LocomotionAxis();
        Vector3 CursorDisplacement();
        bool RotationCameraDH();
        float LeftRotationCamera();
        float RightRotationCamera();
        bool ActionButtonD();
        bool CancelButtonD();
        bool SwitchSelectionButtonD();
        float CameraZoom();
        bool FiringActionDown();
        bool FiringActionDownHold();
        bool FiringProjectileDH();
        bool MenuExitD();
        bool FreezeTimeDown();
    }

    public enum MouseButton
    {
        NONE = 0,
        LEFT_BUTTON = 1,
        RIGHT_BUTTON = 2
    }

    public enum MouseScroll
    {
        NONE = 0,
        SCROLL = 1
    }
}