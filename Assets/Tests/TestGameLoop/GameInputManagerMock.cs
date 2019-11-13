using System.Collections.Generic;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Tests
{
    public class GameTestMockedInputManager : GameInputManager
    {
        public GameTestMockedInputManager()
        {
            this.currentInput = new GameTestMockedXInput();
        }
        public static void SetupForTestScene()
        {
            SetInstance(new GameTestMockedInputManager());
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
        public bool switchSelectionButtonD = false;
        public bool actionButtonD = false;
        public bool timeForwardButtonDH = false;
        public Vector3 locomotionAxis = Vector3.zero;

        #region Interface implementation

        public bool ActionButtonD()
        {
            return actionButtonD;
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
            return this.locomotionAxis;
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
            return this.switchSelectionButtonD;
        }

        public bool TimeForwardButtonDH()
        {
            return this.timeForwardButtonDH;
        }

        public float CameraZoom()
        {
            return 0f;
        }

        public bool FiringActionDown()
        {
            return false;
        }

        public bool FiringActionReleased()
        {
            return false;
        }

        public bool GetInputCondition(InputID InputID)
        {
            return false;
        }

        #endregion
    }
}