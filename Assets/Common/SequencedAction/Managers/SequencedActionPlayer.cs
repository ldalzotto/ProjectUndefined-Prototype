using System;
using System.Collections.Generic;

namespace SequencedAction
{
    public class SequencedActionPlayer
    {
        private SequencedActionManager SequencedActionManager;
        public List<ASequencedAction> SequencedActions;

        private Action OnCutsceneEnded;
        private Action OnCutsceneKilled;

        public SequencedActionPlayer(List<ASequencedAction> SequencedActions, Action OnCutsceneEnded = null, Action OnCutsceneKilled = null)
        {
            this.OnCutsceneEnded = OnCutsceneEnded;
            this.OnCutsceneKilled = OnCutsceneKilled;

            this.SequencedActions = SequencedActions;
            this.SequencedActionManager = new SequencedActionManager(this.OnCutsceneEnded);
        }

        public void Play()
        {
            this.SequencedActionManager.OnAddActions(this.SequencedActions);
        }

        public void Tick(float d)
        {
            if (this.SequencedActionManager.IsPlaying())
            {
                this.SequencedActionManager.Tick(d);
            }
        }

        public void Kill()
        {
            this.SequencedActionManager.InterruptAllActions();
            this.SequencedActionManager.CleatAllActions();
            if (this.OnCutsceneKilled != null)
            {
                this.OnCutsceneKilled.Invoke();
            }
        }

        public bool IsPlaying()
        {
            return this.SequencedActionManager.IsPlaying();
        }

        public List<ASequencedAction> GetCurrentActions(bool includeWorkflowNested = false)
        {
            return this.SequencedActionManager.GetCurrentActions(includeWorkflowNested);
        }
    }
}