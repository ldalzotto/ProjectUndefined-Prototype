using System;
using System.Collections.Generic;
using UnityEngine;

namespace SequencedAction
{
    [Serializable]
    public class CutsceneWorkflowWaitForSecondsAction : ASequencedAction
    {
        [SerializeField] public float SecondsToWait = 0f;

        [NonSerialized] private bool hasEnded;
        [NonSerialized] private float currentTimeElapsed;

        public CutsceneWorkflowWaitForSecondsAction(float SecondsToWait)
        {
            this.SecondsToWait = SecondsToWait;
        }

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
            this.currentTimeElapsed = 0f;
        }

        public override void Tick(float d)
        {
            this.currentTimeElapsed += d;
            this.hasEnded = this.currentTimeElapsed >= SecondsToWait;
        }

        public override void Interupt()
        {
            base.Interupt();
            this.hasEnded = true;
        }
    }
}