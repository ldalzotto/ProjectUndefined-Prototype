using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;
using UnityEngine;

namespace PlayerObject
{
    public class PlayerObjectAnimationStateManager
    {
        private PlayerObjectLocomotionStateBehavior PlayerObjectLocomotionStateBehavior;
        private PlayerLocomotionMaskedPoseOVerrideStateBehavior PlayerLocomotionMaskedPoseOVerrideStateBehavior;

        public PlayerObjectAnimationStateManager(AnimationController animationControllerRef, A_AnimationPlayableDefinition playerLocomotionTree)
        {
            this.PlayerObjectLocomotionStateBehavior = new PlayerObjectLocomotionStateBehavior(PlayerObjectLocomotionState.MOVING, animationControllerRef, playerLocomotionTree);
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior = new PlayerLocomotionMaskedPoseOVerrideStateBehavior(animationControllerRef);
        }

        public void Tick(float d)
        {
            this.PlayerObjectLocomotionStateBehavior.Tick(d);
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior.Tick(d);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.PlayerObjectLocomotionStateBehavior.GetCurrentStateManager().SetUnscaledObjectLocalDirection(localDirection);
        }

        public void StartTargetting(A_AnimationPlayableDefinition startTargettingPoseAnimation)
        {
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior.StartTargetting(startTargettingPoseAnimation);
        }

        public void EndTargetting()
        {
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior.EndTargetting();
        }

        public void OnLowHealthStarted(A_AnimationPlayableDefinition lowHealthLocomotionAnimationTree)
        {
            this.PlayerObjectLocomotionStateBehavior.OnLowHealthStarted(lowHealthLocomotionAnimationTree);
        }

        public void OnLowHealthEnded()
        {
            this.PlayerObjectLocomotionStateBehavior.OnLowHealthEnded();
        }
    }

    public enum PlayerObjectAnimationLayers
    {
        LOCOMOTION,
        TARGETTING_UPPER_BODY_POSE
    }

    public static class PlayerObjectAnimationLayersOrders
    {
        public static int GetLayerNumber(PlayerObjectAnimationLayers PlayerObjectAnimationLayers)
        {
            return PlayerObjectAnimationLayersOrder[PlayerObjectAnimationLayers];
        }

        static Dictionary<PlayerObjectAnimationLayers, int> PlayerObjectAnimationLayersOrder = new Dictionary<PlayerObjectAnimationLayers, int>()
        {
            {PlayerObjectAnimationLayers.LOCOMOTION, 0},
            {PlayerObjectAnimationLayers.TARGETTING_UPPER_BODY_POSE, 1}
        };
    }
}