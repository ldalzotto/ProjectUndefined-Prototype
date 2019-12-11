using AnimatorPlayable;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public class CommonBipedAnimatorBehavior
    {
        private CommonBipedAnimatorBehaviorDefinition CommonBipedAnimatorBehaviorDefinition;
        private AnimationController AnimationControllerRef;

        private Vector2 normalizedObjectSpeed;

        public CommonBipedAnimatorBehavior(AnimationController AnimationController, CommonBipedAnimatorBehaviorDefinition CommonBipedAnimatorBehaviorDefinition)
        {
            this.AnimationControllerRef = AnimationController;
            this.AnimationControllerRef.PlayAnimationV2(0, CommonBipedAnimatorBehaviorDefinition.LocomotionTreeLowerBody.GetAnimationInput(), TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
            this.AnimationControllerRef.PlayAnimationV2(1, CommonBipedAnimatorBehaviorDefinition.LocomotionTreeUpperBody.GetAnimationInput(), TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.normalizedObjectSpeed = new Vector2(localDirection.x, localDirection.z);
        }

        public void StartTargetting(A_AnimationPlayableDefinition startTargettingPoseAnimation)
        {
            this.AnimationControllerRef.PlayAnimationV2(2, startTargettingPoseAnimation.GetAnimationInput());
        }

        public void EndTargetting()
        {
            this.AnimationControllerRef.DestroyAnimationLayerV2(2);
        }
    }
}