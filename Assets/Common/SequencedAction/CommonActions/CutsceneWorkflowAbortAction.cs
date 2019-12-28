using System;
using System.Collections.Generic;

namespace SequencedAction
{
    [Serializable]
    public class CutsceneWorkflowAbortAction : ASequencedAction
    {
        [NonSerialized] private List<ASequencedAction> sequencedActionsToInterrupt;

        public List<ASequencedAction> SequencedActionsToInterrupt
        {
            set => sequencedActionsToInterrupt = value;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction()
        {
            if (this.sequencedActionsToInterrupt != null)
            {
                foreach (var sequencedActionToInterrupt in this.sequencedActionsToInterrupt)
                {
                    if (!sequencedActionToInterrupt.IsFinished())
                    {
                        sequencedActionToInterrupt.Interupt();
                    }
                }
            }
        }

        public override void Tick(float d)
        {
        }
    }
}