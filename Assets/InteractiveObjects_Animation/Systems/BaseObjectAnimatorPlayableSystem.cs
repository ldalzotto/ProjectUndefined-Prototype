using AnimatorPlayable;
using InteractiveObjects_Interfaces;

namespace InteractiveObject_Animation
{
    public class BaseObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private float normalizedObjectSpeed;

        public BaseObjectAnimatorPlayableSystem(AnimatorPlayableObject AnimatorPlayableObject, A_AnimationPlayableDefinition LocomotionAnimationDefinition)
        {
            AnimatorPlayableObject.PlayAnimation(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, LocomotionAnimationDefinition.GetAnimationInput(),
                InputWeightProvider: () => this.normalizedObjectSpeed);
        }

        public void SetUnscaledObjectSpeed(float normalizedObjectSpeed)
        {
            this.normalizedObjectSpeed = normalizedObjectSpeed;
        }
    }
}