using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public class BaseObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private AnimationController AnimationControllerRef;

        private Vector2 normalizedObjectSpeed;

        public BaseObjectAnimatorPlayableSystem(AnimationController AnimationController, A_AnimationPlayableDefinition UpperBodyLocomotion, A_AnimationPlayableDefinition LowerBodyLocomotion)
        {
            this.AnimationControllerRef = AnimationController;
            this.AnimationControllerRef.PlayLocomotionAnimationOverride(UpperBodyLocomotion.GetAnimationInput(), AnimationLayerID.LocomotionLayer, TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
            this.AnimationControllerRef.PlayLocomotionAnimationOverride(LowerBodyLocomotion.GetAnimationInput(), AnimationLayerID.LocomotionLayer_Lower, TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
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