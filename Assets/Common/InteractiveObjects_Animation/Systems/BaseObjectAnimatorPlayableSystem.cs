using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public class BaseObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private AnimationController AnimationControllerRef;

        private Vector2 normalizedObjectSpeed;

        public BaseObjectAnimatorPlayableSystem(AnimationController AnimationController, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            this.AnimationControllerRef = AnimationController;
            this.AnimationControllerRef.PlayLocomotionAnimation(LocomotionAnimationDefinition.GetAnimationInput(), TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public void PlayLocomotionAnimationOverride(A_AnimationPlayableDefinition LocomotionAnimationDefinition, AnimationLayerID overrideLayer)
        {
            this.AnimationControllerRef.PlayLocomotionAnimationOverride(LocomotionAnimationDefinition.GetAnimationInput(), overrideLayer, TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.normalizedObjectSpeed = new Vector2(localDirection.x, localDirection.z);
        }
    }
}