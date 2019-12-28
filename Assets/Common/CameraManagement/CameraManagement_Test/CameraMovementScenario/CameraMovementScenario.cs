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
                                    .Then(new AfterCursorMovementAction()
                                        .Then(new WaitForSecondsAction(1f)
                                            .Then(new ResetRotationInputAction()
                                                .Then(new WaitForSecondsAction(1f))
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

    class AfterCursorMovementAction : ASequencedAction
    {
        public override void FirstExecutionAction()
        {
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RotationCameraDH = true;
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RightRotationCamera = 100f;
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

    class ResetRotationInputAction : ASequencedAction
    {
        public override void FirstExecutionAction()
        {
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RotationCameraDH = false;
            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RightRotationCamera = 0f;
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