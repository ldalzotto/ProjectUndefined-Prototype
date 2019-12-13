using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;
using UnityEngine;

namespace PlayerObject
{
    #region PlayerLocomotion

    public enum PlayerObjectLocomotionState
    {
        LISTENING,
        MOVING,
        MOVING_INJURED
    }

    class PlayerObjectLocomotionStateBehavior : StateBehavior<PlayerObjectLocomotionState, PlayerObjectLocomotionStateManager>
    {
        private PlayerLocomotionSystem playerLocomotionSystem;

        public PlayerObjectLocomotionStateBehavior(PlayerObjectLocomotionState StartState, AnimationController animationControllerRef, A_AnimationPlayableDefinition playerLocomotionTree)
        {
            this.playerLocomotionSystem = new PlayerLocomotionSystem(playerLocomotionTree);
            base.StateManagersLookup = new Dictionary<PlayerObjectLocomotionState, PlayerObjectLocomotionStateManager>()
            {
                {PlayerObjectLocomotionState.LISTENING, new PlayerObjectLocomotionDummyStateManager()},
                {PlayerObjectLocomotionState.MOVING, new PlayerObjectLocomotionMovingStatemanager(this.playerLocomotionSystem, animationControllerRef)},
                {PlayerObjectLocomotionState.MOVING_INJURED, new PlayerObjectLocomotionMovingInjuredStatemanager(this.playerLocomotionSystem, animationControllerRef)}
            };

            base.Init(PlayerObjectLocomotionState.LISTENING);
            this.SetState(StartState);
        }

        public void OnLowHealthStarted(A_AnimationPlayableDefinition lowHealthLocomotionAnimationTree)
        {
            this.playerLocomotionSystem.OnLowHealthStarted(lowHealthLocomotionAnimationTree);
            this.SetState(PlayerObjectLocomotionState.MOVING_INJURED);
        }

        public void OnLowHealthEnded()
        {
            this.SetState(PlayerObjectLocomotionState.MOVING);
        }
    }


    public abstract class PlayerObjectLocomotionStateManager : StateManager
    {
        public virtual void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
        }
    }

    class PlayerObjectLocomotionDummyStateManager : PlayerObjectLocomotionStateManager
    {
    }

    class PlayerObjectLocomotionMovingStatemanager : PlayerObjectLocomotionStateManager
    {
        private PlayerLocomotionSystem _playerLocomotionSystem;
        private AnimationController AnimationControllerRef;

        public PlayerObjectLocomotionMovingStatemanager(PlayerLocomotionSystem playerLocomotionSystem, AnimationController animationControllerRef)
        {
            this._playerLocomotionSystem = playerLocomotionSystem;
            AnimationControllerRef = animationControllerRef;
        }

        public override void OnStateEnter()
        {
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), this._playerLocomotionSystem.DefaultLocomotionAnimation.GetAnimationInput());
        }

        public override void OnStateExit()
        {
            this.AnimationControllerRef.DestroyAnimationLayerV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION));
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.AnimationControllerRef.SetTwoDInputWeight(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), new Vector2(localDirection.x, localDirection.z));
        }
    }

    class PlayerObjectLocomotionMovingInjuredStatemanager : PlayerObjectLocomotionStateManager
    {
        private PlayerLocomotionSystem _playerLocomotionSystem;
        private AnimationController AnimationControllerRef;

        public PlayerObjectLocomotionMovingInjuredStatemanager(PlayerLocomotionSystem playerLocomotionSystem, AnimationController animationControllerRef)
        {
            this._playerLocomotionSystem = playerLocomotionSystem;
            AnimationControllerRef = animationControllerRef;
        }

        public override void OnStateEnter()
        {
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), this._playerLocomotionSystem.InjuredLocomotionAnimation.GetAnimationInput());
        }

        public override void OnStateExit()
        {
            this.AnimationControllerRef.DestroyAnimationLayerV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION));
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.AnimationControllerRef.SetTwoDInputWeight(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), new Vector2(localDirection.x, localDirection.z));
        }
    }

    #endregion
}