using System;
using System.Collections.Generic;

namespace SequencedAction
{
    [Serializable]
    public class BranchInfiniteLoopAction : ASequencedAction
    {
        [NonSerialized] private List<ASequencedAction> loopActions;
        [NonSerialized] private SequencedActionPlayer loopActionPlayer;

        public BranchInfiniteLoopAction(List<ASequencedAction> loopActions) : base(null)
        {
            this.loopActions = loopActions;
        }

        public override void SetNextContextAction(List<ASequencedAction> nextActions)
        {
            this.loopActions = nextActions;
            base.SetNextContextAction(new List<ASequencedAction>());
        }

        [NonSerialized] private bool hasEnded;

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.hasEnded;
        }

        public override void FirstExecutionAction()
        {
            this.hasEnded = false;
            this.CreateAndPlayLoopAction();
        }

        private void CreateAndPlayLoopAction()
        {
            this.loopActionPlayer = new SequencedActionPlayer(this.loopActions, OnCutsceneEnded: () => { this.CreateAndPlayLoopAction(); });
            this.loopActionPlayer.Play();
        }

        public override void Tick(float d)
        {
            if (this.loopActionPlayer != null)
            {
                this.loopActionPlayer.Tick(d);
            }
        }

        public override void Interupt()
        {
            this.hasEnded = true;
            if (this.loopActionPlayer != null)
            {
                this.loopActionPlayer.Kill();
            }
        }


        #region Data Retrieval

        public List<ASequencedAction> GetCurrentActions(bool includeWorkflowNested = false)
        {
            return this.loopActionPlayer.GetCurrentActions(includeWorkflowNested);
        }

        #endregion
    }
}