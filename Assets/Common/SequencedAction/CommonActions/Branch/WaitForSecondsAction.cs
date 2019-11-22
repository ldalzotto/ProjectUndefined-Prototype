using System;
using System.Collections.Generic;

namespace SequencedAction
{
    [Serializable]
    public class WaitForSecondsAction : ASequencedAction
    {
        [NonSerialized] private float secondsToWait;
        private float currentTimer;
        public WaitForSecondsAction(float SecondsToWait, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.secondsToWait = SecondsToWait;
        }

        public override void FirstExecutionAction()
        {
            this.currentTimer = 0f;
        }

        public override bool ComputeFinishedConditions()
        {
            return this.currentTimer >= this.secondsToWait;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
            this.currentTimer += d;
        }
    }
}