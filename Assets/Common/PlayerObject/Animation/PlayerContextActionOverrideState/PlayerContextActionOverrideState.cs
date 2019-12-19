using System;
using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;

namespace PlayerObject
{
    public enum PlayerContextActionOverrideState
    {
        LISTENING = 0,
        DEFLECTION_MOVEMENT = 1
    }

    public abstract class PlayerContextActionOverrideStateManager : StateManager
    {
    }

    class PlayerContextActionOverrideSystem
    {
        public A_AnimationPlayableDefinition ProjectileDeflectMovementAnimation;
    }

    /// <summary>
    /// The <see cref="PlayerLocomotionMaskedPoseOVerrideStateBehavior"/> goes on the next layer of <see cref="PlayerObjectLocomotionStateBehavior"/>.
    /// It can overrides some parts of the locomotion animation.
    /// </summary>
    public class PlayerContextActionOverrideStateBehavior : StateBehavior<PlayerContextActionOverrideState, PlayerContextActionOverrideStateManager>
    {
        private PlayerContextActionOverrideSystem PlayerContextActionOverrideSystem;

        public PlayerContextActionOverrideStateBehavior(AnimationController AnimationControllerRef)
        {
            this.PlayerContextActionOverrideSystem = new PlayerContextActionOverrideSystem();
            base.StateManagersLookup = new Dictionary<PlayerContextActionOverrideState, PlayerContextActionOverrideStateManager>()
            {
                {PlayerContextActionOverrideState.LISTENING, new ListeningPlayerContextActionOverrideStateManager()},
                {PlayerContextActionOverrideState.DEFLECTION_MOVEMENT, new DeflectionMovementListeningPlayerContextActionOverrideStateManager(AnimationControllerRef,
                    this.PlayerContextActionOverrideSystem, this.OnProjectileDeflectMovementAnimationEnd)}
            };
            base.Init(PlayerContextActionOverrideState.LISTENING);
        }

        public void OnProjectileDeflectionAttempt(A_AnimationPlayableDefinition ProjectileDeflectMovementAnimation)
        {
            this.PlayerContextActionOverrideSystem.ProjectileDeflectMovementAnimation = ProjectileDeflectMovementAnimation;
            this.SetState(PlayerContextActionOverrideState.DEFLECTION_MOVEMENT);
        }

        private void OnProjectileDeflectMovementAnimationEnd()
        {
            this.SetState(PlayerContextActionOverrideState.LISTENING);
        }
    }

    class ListeningPlayerContextActionOverrideStateManager : PlayerContextActionOverrideStateManager
    {
    }

    class DeflectionMovementListeningPlayerContextActionOverrideStateManager : ListeningPlayerContextActionOverrideStateManager
    {
        private AnimationController AnimationControllerRef;

        private PlayerContextActionOverrideSystem PlayerContextActionOverrideSystem;
        #region Callback

        private Action OnProjectileDeflectMovementAnimationEnd;

        #endregion

        public DeflectionMovementListeningPlayerContextActionOverrideStateManager(AnimationController animationControllerRef, 
            PlayerContextActionOverrideSystem PlayerContextActionOverrideSystem, Action OnProjectileDeflectMovementAnimationEnd)
        {
            AnimationControllerRef = animationControllerRef;
            this.PlayerContextActionOverrideSystem = PlayerContextActionOverrideSystem;
            this.OnProjectileDeflectMovementAnimationEnd = OnProjectileDeflectMovementAnimationEnd;
        }

        public override void OnStateEnter()
        {
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.DEFLECT_MOVEMENT_RIGHT_ARM),
                this.PlayerContextActionOverrideSystem.ProjectileDeflectMovementAnimation.GetAnimationInput(), OnAnimationEnd: this.OnProjectileDeflectMovementAnimationEnd);
        }

        public override void OnStateExit()
        {
            this.AnimationControllerRef.StopAnimationLayer(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.DEFLECT_MOVEMENT_RIGHT_ARM));
        }
    }
}