using System;
using System.Collections;
using System.Collections.Generic;
using Firing;
using PlayerActions;
using SequencedAction;
using Tests;
using Tests.TestScenario;

namespace UnityEngine
{
    [Serializable]
    public class TargettingFunctionalityTestScenario : ATestScenarioDefinition
    {
        public override List<ASequencedAction> BuildScenarioActions()
        {
            return new List<ASequencedAction>()
            {
                new InputTestAction(null)
            };
        }
    }

    class InputTestAction : ASequencedAction
    {
        public InputTestAction(Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
        }

        public override void FirstExecutionAction()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDown = true;
            Coroutiner.Instance.StartCoroutine(this.Enumeraa());
        }

        public override bool ComputeFinishedConditions()
        {
            return false;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }

        private IEnumerator Enumeraa()
        {
            yield return new WaitForEndOfFrame();
            (PlayerActionManager.Get().GetCurrentlyPlayingPlayerAction() as FiringPlayerAction).SetTargetCursorPosition(Vector2.zero);            
        }
    }
}