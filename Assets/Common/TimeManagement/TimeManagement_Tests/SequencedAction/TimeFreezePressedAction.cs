using System.Collections;
using SequencedAction;
using Tests;
using UnityEngine;

namespace DefaultNamespace
{
    public class TimeFreezePressedAction : ASequencedAction
    {
        private Coroutine routine;

        public override void FirstExecutionAction()
        {
            this.hasEnded = false;
            this.routine = Coroutiner.Instance.StartCoroutine(Routine());
        }

        private bool hasEnded;

        public override bool ComputeFinishedConditions()
        {
            return this.hasEnded;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Interupt()
        {
            base.Interupt();
            if (this.routine != null)
            {
                Coroutiner.Instance.StopCoroutine(this.routine);
            }
        }

        public override void Tick(float d)
        {
        }

        private IEnumerator Routine()
        {
            yield return new WaitForFixedUpdate();
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FreezeTimeDown = true;
            yield return new WaitForEndOfFrame();
            yield return new WaitForFixedUpdate();
            GameTestMockedInputManager.MockedInstance.GetGameTestMockedXInput().GameTestInputMockedValues.FreezeTimeDown = false;
            this.hasEnded = true;
        }
    }
}