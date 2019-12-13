using AnimatorPlayable;

namespace PlayerObject
{
    /// <summary>
    /// Holds all locomotion animation definition graphs
    /// </summary>
    class PlayerLocomotionSystem
    {
        public A_AnimationPlayableDefinition DefaultLocomotionAnimation { get; private set; }
        public A_AnimationPlayableDefinition InjuredLocomotionAnimation { get; private set; }

        public PlayerLocomotionSystem(A_AnimationPlayableDefinition defaultLocomotionAnimation)
        {
            DefaultLocomotionAnimation = defaultLocomotionAnimation;
        }

        public void OnLowHealthStarted(A_AnimationPlayableDefinition lowHealthLocomotionAnimationTree)
        {
            this.InjuredLocomotionAnimation = lowHealthLocomotionAnimationTree;
        }
    }
}