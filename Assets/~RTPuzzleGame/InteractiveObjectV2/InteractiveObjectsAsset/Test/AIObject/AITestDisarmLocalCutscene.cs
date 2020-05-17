using System;
using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;
using InteractiveObjects;
using SequencedAction;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "AITestDisarmLocalCutscene", menuName = "Test/AITestDisarmLocalCutscene")]
    public class AITestDisarmLocalCutscene : LocalPuzzleCutsceneTemplate
    {
        public PlayContextAnimationActionInput BaseAnimationInput;
        public float RepeatWaitForSeconds;

        public override List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject)
        {
            return new List<ASequencedAction>()
            {
                new BranchInfiniteLoopAction(new List<ASequencedAction>()
                {
                    new PlayContextAnimationAction(associatedInteractiveObject.AnimationController, this.BaseAnimationInput, () => new List<ASequencedAction>()
                    {
                        new CutsceneWorkflowWaitForSecondsAction(this.RepeatWaitForSeconds, null)
                    })
                })
            };
        }
    }
}