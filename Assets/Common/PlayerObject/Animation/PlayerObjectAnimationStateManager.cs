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
        private PlayerContextActionOverrideStateBehavior PlayerContextActionOverrideStateBehavior;

        public PlayerObjectAnimationStateManager(AnimationController animationControllerRef, A_AnimationPlayableDefinition playerLocomotionTree)
        {
            this.PlayerObjectLocomotionStateBehavior = new PlayerObjectLocomotionStateBehavior(PlayerObjectLocomotionState.MOVING, animationControllerRef, playerLocomotionTree);
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior = new PlayerLocomotionMaskedPoseOVerrideStateBehavior(animationControllerRef);
            this.PlayerContextActionOverrideStateBehavior = new PlayerContextActionOverrideStateBehavior(animationControllerRef);
        }

        public void Tick(float d)
        {
            this.PlayerObjectLocomotionStateBehavior.Tick(d);
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior.Tick(d);
            this.PlayerContextActionOverrideStateBehavior.Tick(d);
        }

        public void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.PlayerObjectLocomotionStateBehavior.GetCurrentStateManager().SetUnscaledObjectLocalDirection(localDirection);
        }

        public void StartAiming(A_AnimationPlayableDefinition startTargettingPoseAnimation)
        {
            this.PlayerLocomotionMaskedPoseOVerrideStateBehavior.StartAiming(startTargettingPoseAnimation);
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
        
        public void OnProjectileDeflectionAttempt(A_AnimationPlayableDefinition ProjectileDeflectMovementAnimation)
        {
            this.PlayerContextActionOverrideStateBehavior.OnProjectileDeflectionAttempt(ProjectileDeflectMovementAnimation);
        }
    }

    public enum PlayerObjectAnimationLayers
    {
        LOCOMOTION,
        TARGETTING_UPPER_BODY_POSE,
        DEFLECT_MOVEMENT_RIGHT_ARM,
        LOW_HEALTH_VISUAL_EFFET
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
            {PlayerObjectAnimationLayers.TARGETTING_UPPER_BODY_POSE, 1},
            {PlayerObjectAnimationLayers.DEFLECT_MOVEMENT_RIGHT_ARM, 2},
            {PlayerObjectAnimationLayers.LOW_HEALTH_VISUAL_EFFET, 999},
        };
    }
}