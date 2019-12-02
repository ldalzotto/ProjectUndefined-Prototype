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

        public override List<ASequencedAction> BuildScenarioActions()
        {
            return new List<ASequencedAction>()
            {
                new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.5f, 0.5f), new Vector2(0.9f, 0.9f), this.CursorSpeed), () =>
                {
                    return new List<ASequencedAction>()
                    {
                        new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.9f, 0.9f), new Vector2(0.1f, 0.9f), this.CursorSpeed), () =>
                        {
                            return new List<ASequencedAction>()
                            {
                                new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.1f, 0.9f), new Vector2(0.1f, 0.1f), this.CursorSpeed), () =>
                                {
                                    return new List<ASequencedAction>()
                                    {
                                        new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.1f), this.CursorSpeed), () =>
                                        {
                                            return new List<ASequencedAction>()
                                            {
                                                new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.9f, 0.1f), new Vector2(0.5f, 0.8f), this.CursorSpeed), () =>
                                                {
                                                    (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RotationCameraDH = true;
                                                    (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RightRotationCamera = 100f;
                                                    return new List<ASequencedAction>()
                                                    {
                                                        new WaitForSecondsAction(1f, () =>
                                                        {
                                                            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RotationCameraDH = false;
                                                            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.RightRotationCamera = 0f;
                                                            (GameTestMockedInputManager.Get() as GameTestMockedInputManager).GetGameTestMockedXInput().GameTestInputMockedValues.CameraZoomDelta = -10f;
                                                            return new List<ASequencedAction>()
                                                            {
                                                                new WaitForSecondsAction(1f, null)
                                                            };
                                                        })
                                                    };
                                                })
                                            };
                                        })
                                    };
                                })
                            };
                        })
                    };
                })
            };
        }
    }
}