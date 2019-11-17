using System;
using System.Collections;
using System.Collections.Generic;
using Firing;
using InteractiveObjects;
using PlayerActions;
using SequencedAction;
using Targetting;
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

    internal class InputTestAction : ASequencedAction
    {
        public const string TestTargettedObjectName = "Test_Targetted_Object";
        public InputTestAction(Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
        }

        public override void FirstExecutionAction()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDown = true;
            Coroutiner.Instance.StartCoroutine(this.InputTestAction_Enumerator());
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

        private IEnumerator InputTestAction_Enumerator()
        {
            yield return null;
            CoreInteractiveObject targettedObject = null;
            foreach (var io in InteractiveObjects.InteractiveObjectV2Manager.Get().InteractiveObjects)
            {
                if (io.InteractiveGameObject.GetAssociatedGameObjectName() == TestTargettedObjectName)
                {
                    targettedObject = io;
                }
            }
            TargetCursorManager.Get().SetTargetCursorPosition(Camera.main.WorldToScreenPoint(
                targettedObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(targettedObject.InteractiveGameObject.AverageModelLocalBounds.Bounds.center)));
            
            yield return null;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringPorjectileDH = true;
        }
    }
}