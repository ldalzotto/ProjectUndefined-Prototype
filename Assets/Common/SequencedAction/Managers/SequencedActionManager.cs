using System;
using System.Collections.Generic;

namespace SequencedAction
{
    public class SequencedActionManager
    {
        private List<ASequencedAction> ExecutedActions = new List<ASequencedAction>();
        private List<ASequencedAction> FinishedActions = new List<ASequencedAction>();
        private List<ASequencedAction> CurrentNextActions = new List<ASequencedAction>();

        private Action OnNoMoreActionToPlay;

        public SequencedActionManager(Action OnNoMoreActionToPlay = null)
        {
            this.OnNoMoreActionToPlay = OnNoMoreActionToPlay;
        }

        public void Tick(float d, bool eventsTriggered = true)
        {
            foreach (var action in ExecutedActions)
            {
                ProcessTick(d, action);
                if (action.IsFinished())
                {
                    FinishedActions.Add(action);
                }
            }

            foreach (var finishedAction in FinishedActions)
            {
                if (finishedAction.GetNextActions() != null)
                {
                    CurrentNextActions.AddRange(finishedAction.GetNextActions());
                }

                finishedAction.ResetState();
                ExecutedActions.Remove(finishedAction);
            }

            FinishedActions.Clear();

            if (CurrentNextActions.Count > 0)
            {
                foreach (var nextContextAction in CurrentNextActions)
                {
                    this.OnAddAction(nextContextAction);
                }

                CurrentNextActions.Clear();

                //first tick for removing at the same frame if necessary
                this.Tick(0f, eventsTriggered: false);
            }

            if (eventsTriggered)
            {
                if (ExecutedActions.Count == 0)
                {
                    if (this.OnNoMoreActionToPlay != null)
                    {
                        this.OnNoMoreActionToPlay.Invoke();
                    }
                }
            }
        }

        private void ProcessTick(float d, ASequencedAction contextAction)
        {
            contextAction.OnTick(d);
        }


        #region External Events

        public void OnAddAction(ASequencedAction action)
        {
            action.ResetState();
            action.FirstExecutionAction();
            ExecutedActions.Add(action);
            //first tick for removing at the same frame if necessary
            ProcessTick(0f, action);
        }

        public void OnAddActions(List<ASequencedAction> actions)
        {
            foreach (var action in actions)
            {
                this.OnAddAction(action);
            }
        }

        public void InterruptAllActions()
        {
            foreach (var action in ExecutedActions)
            {
                action.Interupt();
            }

            this.ExecutedActions.Clear();
        }

        public void CleatAllActions()
        {
            this.ExecutedActions.Clear();
            this.FinishedActions.Clear();
            this.CurrentNextActions.Clear();
        }

        #endregion

        #region Data Retrieval

        public List<ASequencedAction> GetCurrentActions(bool includeWorkflowNested = false)
        {
            if (includeWorkflowNested)
            {
                var returnActions = new List<ASequencedAction>();
                foreach (var executedAction in this.ExecutedActions)
                {
                    if (executedAction.GetType() == typeof(BranchInfiniteLoopAction))
                    {
                        var BranchInfiniteLoopAction = (BranchInfiniteLoopAction) executedAction;
                        returnActions.AddRange(BranchInfiniteLoopAction.GetCurrentActions(includeWorkflowNested));
                    }

                    returnActions.Add(executedAction);
                }

                return returnActions;
            }
            else
            {
                return this.ExecutedActions;
            }
        }

        public bool IsPlaying()
        {
            return this.GetCurrentActions().Count > 0;
        }

        #endregion
    }
}