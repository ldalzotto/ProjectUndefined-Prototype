using System.Collections.Generic;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Tests
{
    public class GameTestMockedInputManager : GameInputManager
    {
        public static GameTestMockedInputManager MockedInstance;
        public GameTestMockedInputManager()
        {
            this.currentInput = new GameTestMockedXInput();
        }

        public static void SetupForTestScene()
        {
            MockedInstance = new GameTestMockedInputManager();
            SetInstance(MockedInstance);
        }

        public override void Init(CursorLockMode CursorLockMode)
        {
            this.currentInput = new GameTestMockedXInput();
        }

        public override Dictionary<Key, KeyControl> GetKeyToKeyControlLookup()
        {
            return new Dictionary<Key, KeyControl>();
        }

        public GameTestMockedXInput GetGameTestMockedXInput()
        {
            return (GameTestMockedXInput) this.currentInput;
        }
    }

    public class GameTestMockedXInput : XInput
    {
        public GameTestInputMockedValues GameTestInputMockedValues = new GameTestInputMockedValues();
        
        #region Interface implementation

        public bool ActionButtonD()
        {
            return GameTestInputMockedValues.ActionButtonD;
        }

        public bool CancelButtonD()
        {
            return false;
        }

        public bool CancelButtonDH()
        {
            return false;
        }

        public Vector3 CursorDisplacement()
        {
            return Vector3.zero;
        }

        public bool InventoryButtonD()
        {
            return false;
        }

        public float LeftRotationCameraDH()
        {
            return 0f;
        }

        public Vector3 LocomotionAxis()
        {
            return this.GameTestInputMockedValues.LocomotionAxis;
        }

        public bool PuzzleResetButton()
        {
            return false;
        }

        public float RightRotationCameraDH()
        {
            return 0f;
        }

        public bool SwitchSelectionButtonD()
        {
            return this.GameTestInputMockedValues.SwitchSelectionButtonD;
        }

        public bool TimeForwardButtonDH()
        {
            return this.GameTestInputMockedValues.TimeForwardButtonDH;
        }

        public float CameraZoom()
        {
            return 0f;
        }

        public bool FiringActionDown()
        {
            return this.GameTestInputMockedValues.FiringActionDown;
        }

        public bool FiringActionReleased()
        {
            return false;
        }

        public bool FiringProjectileDH()
        {
            return this.GameTestInputMockedValues.FiringPorjectileDH;
        }

        public bool MenuExitD()
        {
            return false;
        }

        #endregion
    }

    public class GameTestInputMockedValues
    {
        public bool SwitchSelectionButtonD;
        public bool ActionButtonD;
        public bool TimeForwardButtonDH;
        public Vector3 LocomotionAxis;
        public bool FiringActionDown;
        public bool FiringPorjectileDH;

        public GameTestInputMockedValues()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.SwitchSelectionButtonD = false;
            this.ActionButtonD = false;
            this.TimeForwardButtonDH = false;
            this.LocomotionAxis = Vector3.zero;
            this.FiringActionDown = false;
            this.FiringPorjectileDH = false;
        }
    }
}