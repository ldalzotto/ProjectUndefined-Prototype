using System;
using System.Collections;
using SequencedAction;
using Targetting_Test;
using Tests;
using Tests.TestScenario;
using UnityEngine;

namespace PlayerDash_Tests
{
    [Serializable]
    public class PlayerDashTestScenario : ATestScenarioDefinition
    {
        public override ASequencedAction[] BuildScenarioActions()
        {
            return new ASequencedAction[]
            {
                new PressDashSkillInput()
                    .Then(new MoveTargetCursorSmoothScreenPosition(new MoveTargetCursorSmoothScreenPositionData(new Vector2(0.5f, 0.5f), new Vector2(0.7f, 0.5f), 0.1f))
                        .Then(new PressDashSkillInput()))
            };
        }
    }

    class PressDashSkillInput : ASequencedAction
    {
        private bool Ended = false;
        private Coroutine routine;

        public override void FirstExecutionAction()
        {
            this.Ended = false;
            this.routine = Coroutiner.Instance.StartCoroutine(this.PressInput());
        }

        public override bool ComputeFinishedConditions()
        {
            return this.Ended;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }

        private IEnumerator PressInput()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.Skil21DownHold = true;
            yield return new WaitForEndOfFrame();
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.Skil21DownHold = false;
            this.Ended = true;
        }
    }
}