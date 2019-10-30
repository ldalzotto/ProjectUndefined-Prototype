using System;
using System.Collections.Generic;
using AnimatorPlayable;
using SequencedAction;

namespace InteractiveObject_Animation
{
    [Serializable]
    public class PlayContextAnimationActionInput
    {
        public bool RootMotion;
        public SequencedAnimationInput SequencedAnimationInput;
    }

    public class PlayContextAnimationAction : ASequencedAction
    {
        private AnimationController AnimationController;
        private PlayContextAnimationActionInput PlayContextAnimationActionInput;

        private bool AnimationFinished;

        public PlayContextAnimationAction(AnimationController AnimationController, PlayContextAnimationActionInput PlayContextAnimationActionInput, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.AnimationController = AnimationController;
            this.PlayContextAnimationActionInput = PlayContextAnimationActionInput;
            this.AnimationFinished = false;
        }

        public override void FirstExecutionAction()
        {
            this.AnimationFinished = false;
            this.AnimationController.PlayContextAction(this.PlayContextAnimationActionInput.SequencedAnimationInput, this.PlayContextAnimationActionInput.RootMotion,
                () => { this.AnimationFinished = true; });
        }

        public override bool ComputeFinishedConditions()
        {
            return this.AnimationFinished;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }

        public override void Interupt()
        {
            this.AnimationController.KillContextAction(this.PlayContextAnimationActionInput.SequencedAnimationInput);
            base.Interupt();
        }
    }
}