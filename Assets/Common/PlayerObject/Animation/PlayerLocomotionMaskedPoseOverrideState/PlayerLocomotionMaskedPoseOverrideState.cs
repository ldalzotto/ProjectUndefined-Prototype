using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObject_Animation;

namespace PlayerObject
{
    #region PlayerLocomotionMaskedPoseOVerride

    public enum PlayerLocomotionMaskedPoseOVerrideState
    {
        LISTENING = 0,
        TARGETTING_UPPER_BODY = 1
    }


    public abstract class PlayerLocomotionMaskedPoseOverrideStateManager : StateManager
    {
    }

    /// <summary>
    /// The <see cref="PlayerLocomotionMaskedPoseOVerrideStateBehavior"/> goes on the next layer of <see cref="PlayerObjectLocomotionStateBehavior"/>.
    /// It can overrides some parts of the locomotion animation.
    /// </summary>
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
            this.AnimationControllerRef.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.TARGETTING_UPPER_BODY_POSE), this.OverridingSystem.StartTargettingPoseAnimation.GetAnimationInput());
        }

        public override void OnStateExit()
        {
            this.AnimationControllerRef.StopAnimationLayer(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.TARGETTING_UPPER_BODY_POSE));
        }
    }

    #endregion
}