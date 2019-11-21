using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public class BaseObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private Vector2 normalizedObjectSpeed;

        public BaseObjectAnimatorPlayableSystem(AnimationController AnimationController, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            AnimationController.PlayLocomotionAnimation(LocomotionAnimationDefinition.GetAnimationInput(), TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.normalizedObjectSpeed = new Vector2(localDirection.x, localDirection.z);
        }
    }
}