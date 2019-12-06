using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveObjects;
using SequencedAction;
using Targetting;
using Tests;
using UnityEngine;

namespace Targetting_Test
{
    public static class TargetAndFireTestUtil
    {
        public static void SetupInputForStoppingFiring()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDownHold = false;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDown = false;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringPorjectileDH = false;
        }

        public static void SetupInputForStartingFiring()
        {
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDownHold = true;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringActionDown = true;
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FiringPorjectileDH = true;
        }
    }

    /// <summary>
    /// Fire a single projectile at the designed target
    /// </summary>
    public class TargetAndFireToInteractiveObjectAction : ASequencedAction
    {
        private CoreInteractiveObject Target;

        public TargetAndFireToInteractiveObjectAction(CoreInteractiveObject Target,
            Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.Target = Target;
        }

        private bool ended;
        private Coroutine Coroutine;

        public override void FirstExecutionAction()
        {
            this.ended = false;
            this.Coroutine = Coroutiner.Instance.StartCoroutine(this.ActionCoroutine());
        }

        public override bool ComputeFinishedConditions()
        {
            return this.ended;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }

        private IEnumerator ActionCoroutine()
        {
            TargetAndFireTestUtil.SetupInputForStartingFiring();
            TargetCursorManager.Get().SetTargetCursorPosition(Camera.main.WorldToScreenPoint(
                this.Target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.Target.InteractiveGameObject.AverageModelLocalBounds.Bounds.center)));
            yield return null;
            TargetAndFireTestUtil.SetupInputForStoppingFiring();
            this.ended = true;
        }


        public override void Interupt()
        {
            base.Interupt();
            Coroutiner.Instance.StopCoroutine(this.Coroutine);
        }
    }

    public class Target_FireInteractiveObject_AndWait_ActionDefintion
    {
        public Func<bool> ManuallyTriggerExitFunction;

        public Target_FireInteractiveObject_AndWait_ActionDefintion(Func<bool> manuallyTriggerExitFunction)
        {
            ManuallyTriggerExitFunction = manuallyTriggerExitFunction;
        }
    }
    /// <summary>
    /// Fire multiple projectile at the target until <see cref="Target_FireInteractiveObject_AndWait_ActionDefintion.ManuallyTriggerExitFunction"/> condition
    /// is met.
    /// </summary>
    public class Target_FireInteractiveObject_AndWait_Action : ASequencedAction
    {
        private CoreInteractiveObject Target;
        private Target_FireInteractiveObject_AndWait_ActionDefintion Target_FireInteractiveObject_AndWait_ActionDefintion;

        public Target_FireInteractiveObject_AndWait_Action(CoreInteractiveObject target,
            Target_FireInteractiveObject_AndWait_ActionDefintion Target_FireInteractiveObject_AndWait_ActionDefintion,
            Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.Target = target;
            this.Target_FireInteractiveObject_AndWait_ActionDefintion = Target_FireInteractiveObject_AndWait_ActionDefintion;
        }

        public override void FirstExecutionAction()
        {
            TargetAndFireTestUtil.SetupInputForStartingFiring();
            TargetCursorManager.Get().SetTargetCursorPosition(Camera.main.WorldToScreenPoint(
                this.Target.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.Target.InteractiveGameObject.AverageModelLocalBounds.Bounds.center)));
        }

        public override bool ComputeFinishedConditions()
        {
            bool ended = this.Target_FireInteractiveObject_AndWait_ActionDefintion.ManuallyTriggerExitFunction.Invoke();
            if (ended)
            {
                TargetAndFireTestUtil.SetupInputForStoppingFiring();
            }

            return ended;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }
    }
}