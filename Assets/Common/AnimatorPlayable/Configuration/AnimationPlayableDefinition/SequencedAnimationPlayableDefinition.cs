using System;

namespace AnimatorPlayable
{
    [Serializable]
    public class SequencedAnimationPlayableDefinition : A_AnimationPlayableDefinition
    {
        public SequencedAnimationInput SequencedAnimationInput;

        public override IAnimationInput GetAnimationInput()
        {
            return this.SequencedAnimationInput;
        }
    }
}