using System.Collections.Generic;
using Input;
using InteractiveObjectAction;

namespace PlayerObject
{
    /// <summary>
    /// The <see cref="PlayerObjectInteractiveObjectActionStateManager"/> is responsible of executing and stopping InteractiveObjectAction that aren't skills
    /// (by calling the <see cref="InteractiveObjectActionPlayerSystem"/>) based on some conditions.
    /// It acts as a Layer between the <see cref="PlayerInteractiveObject"/> and the <see cref="InteractiveObjectActionPlayerSystem"/> by introducing some logical between.
    /// </summary>
    public class PlayerObjectInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;

        public PlayerObjectInteractiveObjectActionStateManager(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData)
        {
            this.FiringInteractiveObjectActionStateBehavior = new FiringInteractiveObjectActionStateBehavior(gameInputManager, interactiveObjectActionPlayerSystem, firingInteractiveObjectActionInherentData);
        }

        public void Tick(float d)
        {
            this.FiringInteractiveObjectActionStateBehavior.Tick(d);
        }

        #region External Events

        public void StopTargetting()
        {
            this.FiringInteractiveObjectActionStateBehavior.GetCurrentStateManager().StopTargetting();
        }

        #endregion

        #region Logical conditions

        public bool IsAiming()
        {
            return this.FiringInteractiveObjectActionStateBehavior.IsAiming();
        }

        #endregion
    }

    #region Aiming

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

    public class ListeningAimingInteractiveObjectActionStateManager : AAimingInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;
        private AimingInteractiveObjectActionStateBehaviorInputDataSystem _aimingInteractiveObjectActionStateBehaviorInputDataSystemRef;

        public ListeningAimingInteractiveObjectActionStateManager(FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehaviorRef,
            ref AimingInteractiveObjectActionStateBehaviorInputDataSystem aimingInteractiveObjectActionStateBehaviorInputDataSystemRef)
        {
            this.FiringInteractiveObjectActionStateBehavior = FiringInteractiveObjectActionStateBehaviorRef;
            this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef = aimingInteractiveObjectActionStateBehaviorInputDataSystemRef;
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.PlayerActionTriggering();
        }

        /// <summary>
        /// Starts a new <see cref="AInteractiveObjectAction"/> if input condition and player inherent conditions are met.
        /// </summary>
        private void PlayerActionTriggering()
        {
            if (this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.GameInputManager.CurrentInput.FiringActionDown())
            {
                this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.InteractiveObjectActionPlayerSystem.ExecuteActionV2
                    (this._aimingInteractiveObjectActionStateBehaviorInputDataSystemRef.firingInteractiveObjectActionInherentData);
                this.FiringInteractiveObjectActionStateBehavior.SetState(AimingInteractiveObjectActionState.AIMING);
            }
        }
    }

    public class AimingAimingInteractiveObjectActionStateManager : AAimingInteractiveObjectActionStateManager
    {
        private FiringInteractiveObjectActionStateBehavior FiringInteractiveObjectActionStateBehavior;

        public AimingAimingInteractiveObjectActionStateManager(FiringInteractiveObjectActionStateBehavior firingInteractiveObjectActionStateBehavior)
        {
            FiringInteractiveObjectActionStateBehavior = firingInteractiveObjectActionStateBehavior;
        }

        public override void StopTargetting()
        {
            this.FiringInteractiveObjectActionStateBehavior.SetState(AimingInteractiveObjectActionState.LISTENING);
        }
    }

    public struct AimingInteractiveObjectActionStateBehaviorInputDataSystem
    {
        public GameInputManager GameInputManager;
        public InteractiveObjectActionPlayerSystem InteractiveObjectActionPlayerSystem;
        public InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData;

        public AimingInteractiveObjectActionStateBehaviorInputDataSystem(GameInputManager gameInputManager, InteractiveObjectActionPlayerSystem interactiveObjectActionPlayerSystem,
            InteractiveObjectActionInherentData firingInteractiveObjectActionInherentData)
        {
            GameInputManager = gameInputManager;
            InteractiveObjectActionPlayerSystem = interactiveObjectActionPlayerSystem;
            this.firingInteractiveObjectActionInherentData = firingInteractiveObjectActionInherentData;
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

    #endregion
}