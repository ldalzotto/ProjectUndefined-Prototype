using System.Collections.Generic;
using AnimatorPlayable;
using DefaultNamespace;
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
        private LocomotionAnimationStateSystem LocomotionAnimationStateSystem;

        private PlayerLocomotionSystem PlayerLocomotionSystem;

        public PlayerObjectLocomotionMovingStatemanager(PlayerLocomotionSystem playerLocomotionSystem, AnimationController animationControllerRef)
        {
            this.LocomotionAnimationStateSystem = new LocomotionAnimationStateSystem(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), animationControllerRef);
            this.PlayerLocomotionSystem = playerLocomotionSystem;
        }

        public override void OnStateEnter()
        {
            this.LocomotionAnimationStateSystem.PlayAnimation(this.PlayerLocomotionSystem.DefaultLocomotionAnimation);
        }

        public override void OnStateExit()
        {
            this.LocomotionAnimationStateSystem.KillAnimation();
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.LocomotionAnimationStateSystem.SetUnscaledObjectLocalDirection(localDirection);
        }
    }

    class PlayerObjectLocomotionMovingInjuredStatemanager : PlayerObjectLocomotionStateManager
    {
        private LocomotionAnimationStateSystem LocomotionAnimationStateSystem;
        private PlayerLocomotionSystem _playerLocomotionSystem;

        public PlayerObjectLocomotionMovingInjuredStatemanager(PlayerLocomotionSystem playerLocomotionSystem, AnimationController animationControllerRef)
        {
            this._playerLocomotionSystem = playerLocomotionSystem;
            this.LocomotionAnimationStateSystem = new LocomotionAnimationStateSystem(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), animationControllerRef);
        }

        public override void OnStateEnter()
        {
            this.LocomotionAnimationStateSystem.PlayAnimation(this._playerLocomotionSystem.InjuredLocomotionAnimation);
        }

        public override void OnStateExit()
        {
            this.LocomotionAnimationStateSystem.KillAnimation();
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.LocomotionAnimationStateSystem.SetUnscaledObjectLocalDirection(localDirection);
        }
    }

    #endregion
}