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
        private GameInputSystemUpdater GameInputSystemUpdater;

        public GameTestMockedInputManager()
        {
            this.GameInputSystemUpdater = new GameInputSystemUpdater();
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

        public override void FixedTick()
        {
            this.GameInputSystemUpdater.FixedTick();
        }

        public override void Tick()
        {
            this.GameInputSystemUpdater.Tick();
        }

        public override void LateTick()
        {
            this.GameInputSystemUpdater.LateTick();
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

        public bool RotationCameraDH()
        {
            return this.GameTestInputMockedValues.RotationCameraDH;
        }

        public bool InventoryButtonD()
        {
            return false;
        }

        public float LeftRotationCamera()
        {
            return 0f;
        }

        public bool EvaluateInputCondition(InputID InputID)
        {
            switch (InputID)
            {
                case InputID.FIRING_PROJECTILE_DOWN_HOLD:
                    return this.FiringProjectileDH();
            }

            return false;
        }

        public Vector3 LocomotionAxis()
        {
            return this.GameTestInputMockedValues.LocomotionAxis;
        }

        public bool PuzzleResetButton()
        {
            return false;
        }

        public float RightRotationCamera()
        {
            return this.GameTestInputMockedValues.RightRotationCamera;
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
            return this.GameTestInputMockedValues.CameraZoomDelta;
        }

        public bool FiringActionDown()
        {
            return this.GameTestInputMockedValues.FiringActionDown;
        }

        public bool FiringActionDownHold()
        {
            return this.GameTestInputMockedValues.FiringActionDownHold;
        }

        public bool FiringProjectileDH()
        {
            return this.GameTestInputMockedValues.FiringPorjectileDH;
        }

        public bool MenuExitD()
        {
            return false;
        }

        public bool FreezeTimeDown()
        {
            return false;
        }

        public bool DeflectProjectileDown()
        {
            return this.GameTestInputMockedValues.DeflectProjectileD;
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
        public bool FiringActionDownHold;
        public bool FiringPorjectileDH;
        public bool RotationCameraDH;
        public float RightRotationCamera;
        public float CameraZoomDelta;
        public bool DeflectProjectileD;

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
            this.FiringActionDownHold = false;
            this.FiringPorjectileDH = false;
            this.RotationCameraDH = false;
            this.RightRotationCamera = 0f;
            this.CameraZoomDelta = 0f;
            this.DeflectProjectileD = false;
        }
    }
}