using System;

namespace AnimatorPlayable
{
    [Serializable]
    public class TwoDBlendTreePlayableDefinition : A_AnimationPlayableDefinition
    {
        public TwoDAnimationInput TwoDAnimationInput;

        public override IAnimationInput GetAnimationInput()
        {
            return this.TwoDAnimationInput;
        }
    }
}