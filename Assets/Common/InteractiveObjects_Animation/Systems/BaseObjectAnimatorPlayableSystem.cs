using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public class BaseObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private Vector2 normalizedObjectSpeed;

        public BaseObjectAnimatorPlayableSystem(AnimatorPlayableObject AnimatorPlayableObject, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, LocomotionAnimationDefinition.GetAnimationInput(),
                TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.normalizedObjectSpeed = new Vector2(localDirection.x, localDirection.z);
        }
    }
}