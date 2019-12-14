using AnimatorPlayable;
using InteractiveObject_Animation;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Plays an animation at the layer <see cref="AnimationLayer"/>.
    /// The purpose of this system is to have a common logic for setting local direction to the animation layer via <see cref="SetUnscaledObjectLocalDirection"/>.
    /// </summary>
    public struct LocomotionAnimationStateSystem
    {
        private int AnimationLayer;
        private AnimationController AnimationControllerRef;

        public LocomotionAnimationStateSystem(int animationLayer, AnimationController animationControllerRef) : this()
        {
            AnimationLayer = animationLayer;
            AnimationControllerRef = animationControllerRef;
        }

        public void PlayAnimation(A_AnimationPlayableDefinition LocomotionAnimationTree)
        {
            this.AnimationControllerRef.PlayAnimationV2(AnimationLayer, LocomotionAnimationTree.GetAnimationInput());
        }

        public void KillAnimation()
        {
            this.AnimationControllerRef.DestroyAnimationLayerV2(AnimationLayer);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.AnimationControllerRef.SetTwoDInputWeight(this.AnimationLayer, new Vector2(localDirection.x, localDirection.z));
        }
    }
}