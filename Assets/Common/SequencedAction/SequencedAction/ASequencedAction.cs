using System;
using System.Collections.Generic;

namespace SequencedAction
{
    public abstract class ASequencedAction
    {
        public abstract void FirstExecutionAction();
        public abstract bool ComputeFinishedConditions();
        public abstract void AfterFinishedEventProcessed();
        public abstract void Tick(float d);

        private List<ASequencedAction> NextActions;
        private Func<List<ASequencedAction>> nextActionsDeffered;

        public ASequencedAction(Func<List<ASequencedAction>> nextActionsDeffered)
        {
            this.nextActionsDeffered = nextActionsDeffered;
        }

        public void OnTick(float d)
        {
            if (!isFinished)
            {
                Tick(d);

                if (ComputeFinishedConditions())
                {
                    isFinished = true;
                    AfterFinishedEventProcessed();
                }
            }
        }

        public virtual void Interupt()
        {
            this.isFinished = true;
        }

        private bool isFinished;

        public virtual void SetNextContextAction(List<ASequencedAction> nextActions)
        {
            this.NextActions = nextActions;
        }

        public List<ASequencedAction> GetNextActions()
        {
            if (this.nextActionsDeffered == null)
            {
                return null;
            }

            if (this.NextActions == null)
            {
                this.NextActions = nextActionsDeffered.Invoke();
            }

            return this.NextActions;
        }

        public bool IsFinished()
        {
            return isFinished;
        }

        public void ResetState()
        {
            isFinished = false;
        }
    }
}