using System;
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

    #region PlayerLocomotion

    public enum PlayerObjectLocomotionState
    {
        LISTENING,
        MOVING,
        MOVING_INJURED
    }

    class PlayerObjectLocomotionStateBehavior : StateBehavior<PlayerObjectLocomotionState, PlayerObjectLocomotionStateManager>
    {
        private LocomotionSystem LocomotionSystem;

        public PlayerObjectLocomotionStateBehavior(PlayerObjectLocomotionState StartState, AnimationController animationControllerRef, A_AnimationPlayableDefinition playerLocomotionTree)
        {
            this.LocomotionSystem = new LocomotionSystem();
            this.LocomotionSystem.DefaultLocomotionAnimation = playerLocomotionTree;
            base.StateManagersLookup = new Dictionary<PlayerObjectLocomotionState, PlayerObjectLocomotionStateManager>()
            {
                {PlayerObjectLocomotionState.LISTENING, new PlayerObjectLocomotionDummyStateManager()},
                {PlayerObjectLocomotionState.MOVING, new PlayerObjectLocomotionMovingStatemanager(this.LocomotionSystem, animationControllerRef)},
                {PlayerObjectLocomotionState.MOVING_INJURED, new PlayerObjectLocomotionMovingInjuredStatemanager(this.LocomotionSystem, animationControllerRef)}
            };

            base.Init(PlayerObjectLocomotionState.LISTENING);
            this.SetState(StartState);
        }

        public void OnLowHealthStarted(A_AnimationPlayableDefinition lowHealthLocomotionAnimationTree)
        {
            this.LocomotionSystem.InjuredLocomotionAnimation = lowHealthLocomotionAnimationTree;
            this.SetState(PlayerObjectLocomotionState.MOVING_INJURED);
        }

        public void OnLowHealthEnded()
        {
            this.SetState(PlayerObjectLocomotionState.MOVING);
        }
    }

    class LocomotionSystem
    {
        public A_AnimationPlayableDefinition DefaultLocomotionAnimation { get; set; }
        public A_AnimationPlayableDefinition InjuredLocomotionAnimation { get; set; }
    }

    abstract class PlayerObjectLocomotionStateManager : StateManager
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
        private LocomotionSystem LocomotionSystem;
        private AnimationController AnimationControllerRef;

        public PlayerObjectLocomotionMovingStatemanager(LocomotionSystem LocomotionSystem, AnimationController animationControllerRef)
        {
            this.LocomotionSystem = LocomotionSystem;
            AnimationControllerRef = animationControllerRef;
        }

        private Vector2 normalizedObjectSpeed;

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), this.LocomotionSystem.DefaultLocomotionAnimation.GetAnimationInput(),
                TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            this.AnimationControllerRef.DestroyAnimationLayerV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION));
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.normalizedObjectSpeed = new Vector2(localDirection.x, localDirection.z);
        }
    }

    class PlayerObjectLocomotionMovingInjuredStatemanager : PlayerObjectLocomotionStateManager
    {
        private LocomotionSystem LocomotionSystem;
        private AnimationController AnimationControllerRef;

        public PlayerObjectLocomotionMovingInjuredStatemanager(LocomotionSystem LocomotionSystem, AnimationController animationControllerRef)
        {
            this.LocomotionSystem = LocomotionSystem;
            AnimationControllerRef = animationControllerRef;
        }

        private Vector2 normalizedObjectSpeed;

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION), this.LocomotionSystem.InjuredLocomotionAnimation.GetAnimationInput(),
                TwoDInputWheigtProvider: () => this.normalizedObjectSpeed);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            this.AnimationControllerRef.DestroyAnimationLayerV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOCOMOTION));
        }

        public override void SetUnscaledObjectLocalDirection(Vector3 localDirection)
        {
            this.normalizedObjectSpeed = new Vector2(localDirection.x, localDirection.z);
        }
    }

    #endregion

    #region PlayerLocomotionMaskedPoseOVerride

    public enum PlayerLocomotionMaskedPoseOVerrideState
    {
        LISTENING = 0,
        TARGETTING_UPPER_BODY = 1
    }


    public abstract class PlayerLocomotionMaskedPoseOverrideStateManager : StateManager
    {
    }

    public class PlayerLocomotionMaskedPoseOVerrideStateBehavior : StateBehavior<PlayerLocomotionMaskedPoseOVerrideState, PlayerLocomotionMaskedPoseOverrideStateManager>
    {
        private OverridingSystem OverridingSystem;

        public PlayerLocomotionMaskedPoseOVerrideStateBehavior(AnimationController AnimationControllerRef)
        {
            this.OverridingSystem = new OverridingSystem();
            base.StateManagersLookup = new Dictionary<PlayerLocomotionMaskedPoseOVerrideState, PlayerLocomotionMaskedPoseOverrideStateManager>()
            {
                {PlayerLocomotionMaskedPoseOVerrideState.LISTENING, new ListeningPlayerLocomotionMaskedPoseOverrideStateManager()},
                {PlayerLocomotionMaskedPoseOVerrideState.TARGETTING_UPPER_BODY, new TargettingUpperBodyStateManager(this.OverridingSystem, AnimationControllerRef)}
            };
            base.Init(PlayerLocomotionMaskedPoseOVerrideState.LISTENING);
        }

        public void StartTargetting(A_AnimationPlayableDefinition startTargettingPoseAnimation)
        {
            this.OverridingSystem.StartTargetting(startTargettingPoseAnimation);
            this.SetState(PlayerLocomotionMaskedPoseOVerrideState.TARGETTING_UPPER_BODY);
        }

        public void EndTargetting()
        {
            if (this.GetCurrentState() == PlayerLocomotionMaskedPoseOVerrideState.TARGETTING_UPPER_BODY)
            {
                this.SetState(PlayerLocomotionMaskedPoseOVerrideState.LISTENING);
            }
        }
    }

    public class OverridingSystem
    {
        public A_AnimationPlayableDefinition StartTargettingPoseAnimation { get; private set; }

        public void StartTargetting(A_AnimationPlayableDefinition startTargettingPoseAnimation)
        {
            this.StartTargettingPoseAnimation = startTargettingPoseAnimation;
        }
    }

    public class ListeningPlayerLocomotionMaskedPoseOverrideStateManager : PlayerLocomotionMaskedPoseOverrideStateManager
    {
    }

    public class TargettingUpperBodyStateManager : PlayerLocomotionMaskedPoseOverrideStateManager
    {
        private OverridingSystem OverridingSystem;
        private AnimationController AnimationControllerRef;

        public TargettingUpperBodyStateManager(OverridingSystem overridingSystem, AnimationController animationControllerRef)
        {
            OverridingSystem = overridingSystem;
            AnimationControllerRef = animationControllerRef;
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.TARGETTING_UPPER_BODY_POSE), this.OverridingSystem.StartTargettingPoseAnimation.GetAnimationInput());
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            this.AnimationControllerRef.DestroyAnimationLayerV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.TARGETTING_UPPER_BODY_POSE));
        }
    }

    #endregion
}