using System.Collections.Generic;
using Input;
using InteractiveObjectAction;

namespace PlayerObject
{
    public enum AimingInteractiveObjectActionState
    {
        LISTENING,
        AIMING
    }

    public abstract class AAimingInteractiveObjectActionStateManager : StateManager
    {
        public virtual void StopTargetting()
        {
        }
    }

    public class FiringInteractiveObjectActionStateBehavior : StateBehavior<AimingInteractiveObjectActionState, AAimingInteractiveObjectActionStateManager>
    {
        private AimingInteractiveObjectActionStateBehaviorInputDataSystem _aimingInteractiveObjectActionStateBehaviorInputDataSystem;

        public FiringInteractiveObjectActionStateBehavior(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData)
        {
            this._aimingInteractiveObjectActionStateBehaviorInputDataSystem = new AimingInteractiveObjectActionStateBehaviorInputDataSystem(
                gameInputManager, interactiveObjectActionPlayerSystem, firingInteractiveObjectActionInherentData);
            this.StateManagersLookup = new Dictionary<AimingInteractiveObjectActionState, AAimingInteractiveObjectActionStateManager>()
            {
                {AimingInteractiveObjectActionState.LISTENING, new ListeningAimingInteractiveObjectActionStateManager(this, ref this._aimingInteractiveObjectActionStateBehaviorInputDataSystem)},
                {AimingInteractiveObjectActionState.AIMING, new AimingAimingInteractiveObjectActionStateManager(this)}
            };
            base.Init(AimingInteractiveObjectActionState.LISTENING);
        }

        protected override void Init(AimingInteractiveObjectActionState StartState)
        {
            base.Init(StartState);
        }

        public bool IsAiming()
        {
            return this.GetCurrentState() == AimingInteractiveObjectActionState.AIMING;
        }
    }
}