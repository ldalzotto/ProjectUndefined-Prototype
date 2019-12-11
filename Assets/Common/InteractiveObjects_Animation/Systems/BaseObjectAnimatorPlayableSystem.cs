using AnimatorPlayable;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace InteractiveObject_Animation
{
    public class BaseObjectAnimatorPlayableSystem : AInteractiveObjectSystem
    {
        private AnimationController AnimationControllerRef;

        public BaseObjectAnimatorPlayableSystem(AnimationController AnimationController, A_AnimationPlayableDefinition BipedLocomotion)
        {
            this.AnimationControllerRef = AnimationController;
            this.AnimationControllerRef.PlayAnimationV2(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, BipedLocomotion.GetAnimationInput());
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            Debug.Log(MyLog.Format(new Vector2(localDirection.x, localDirection.z)));
            this.AnimationControllerRef.SetTwoDInputWeight(AnimationLayerStatic.AnimationLayers[AnimationLayerID.LocomotionLayer].ID, new Vector2(localDirection.x, localDirection.z));
        }
    }
}