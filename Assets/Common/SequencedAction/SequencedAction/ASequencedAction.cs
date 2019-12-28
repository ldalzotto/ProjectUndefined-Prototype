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

        private ASequencedAction[] NextActions;

        public ASequencedAction Then(params ASequencedAction[] nextActions)
        {
            this.NextActions = nextActions;
            return this;
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

        public virtual void SetNextContextAction(ASequencedAction[] nextActions)
        {
            this.NextActions = nextActions;
        }

        public ASequencedAction[] GetNextActions()
        {
            if (this.NextActions == null)
            {
                return null;
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