using System;

namespace AnimatorPlayable
{
    [Serializable]
    public class BlendedAnimationPlayableDefinition : A_AnimationPlayableDefinition
    {
        public BlendedAnimationInput BlendedAnimationInput;

        public override IAnimationInput GetAnimationInput()
        {
            return this.BlendedAnimationInput;
        }
    }
}