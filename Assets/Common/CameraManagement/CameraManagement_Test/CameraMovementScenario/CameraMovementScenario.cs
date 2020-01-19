using System;
using System.Collections.Generic;
using SequencedAction;
using Targetting_Test;
using Tests;
using Tests.TestScenario;
using UnityEngine;

namespace CameraManagement_Test
{
    [Serializable]
    public class CameraMovementScenario : ATestScenarioDefinition
    {
        public float CursorSpeed;

        public override ASequencedAction[] BuildScenarioActions()
        {
            return new ASequencedAction[]
            {
                new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.5f, 0.5f), new Vector2(0.9f, 0.9f), this.CursorSpeed))
                    .Then(new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.9f, 0.9f), new Vector2(0.1f, 0.9f), this.CursorSpeed))
                        .Then(new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.1f, 0.9f), new Vector2(0.1f, 0.1f), this.CursorSpeed))
                            .Then(new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.1f), this.CursorSpeed))
                                .Then(new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.9f, 0.1f), new Vector2(0.5f, 0.8f), this.CursorSpeed))
                                    .Then(new RightRotationCameraAction(new RightRotationCameraAction_Input(100f))
                                        .Then(new WaitForSecondsAction(1f)
                                            .Then(new ResetRotationInputAction()
                                                .Then(new CustomCameraZoomAction()
                                                    .Then(new WaitForSecondsAction(1f))
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
            };
        }
    }

    public struct RightRotationCameraAction_Input
    {
        public float RightRotationCameraSpeed;

        public RightRotationCameraAction_Input(float rightRotationCameraSpeed)
        {
            RightRotationCameraSpeed = rightRotationCameraSpeed;
        }
    }

    public class RightRotationCameraAction : ASequencedAction
    {
        private RightRotationCameraAction_Input RightRotationCameraAction_Input;

        public RightRotationCameraAction(RightRotationCameraAction_Input rightRotationCameraActionInput)
        {
            RightRotationCameraAction_Input = rightRotationCameraActionInput;
        }

        public override void FirstExecutionAction()
        {
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RotationCameraDH = true;
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RightRotationCamera = this.RightRotationCameraAction_Input.RightRotationCameraSpeed;
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }
    }

    public class ResetRotationInputAction : ASequencedAction
    {
        public override void FirstExecutionAction()
        {
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RotationCameraDH = false;
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RightRotationCamera = 0f;
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }
    }

    class CustomCameraZoomAction : ASequencedAction
    {
        public override void FirstExecutionAction()
        {
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.CameraZoomDelta = -10f;
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }
    }
}