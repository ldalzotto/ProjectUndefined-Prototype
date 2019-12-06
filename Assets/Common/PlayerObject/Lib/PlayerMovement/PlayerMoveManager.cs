using System;
using Input;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerObject
{
    /// <summary>
    /// Manages <see cref="PlayerInteractiveObject"/> movement from Input <see cref="PlayerRigidBodyMoveManager"/> and from Agent <see cref="PlayerAgentMoveManager"/>.
    /// The choice between Input and Agent is done  by <see cref="PlayerMoveManagerState"/>. <br/>
    /// <see cref="PlayerMoveManagerState"/> values can be changed automatically when a movement input is pressed or when a destination is asked.
    /// </summary>
    public class PlayerMoveManager
    {
        private PlayerMoveManagerState PlayerMoveManagerState;

        private PlayerRigidBodyMoveManager PlayerRigidBodyMoveManager;
        private PlayerAgentMoveManager PlayerAgentMoveManager;

        /// <summary>
        /// The <see cref="APlayerMoveManager"/> that will be used for external calls. It's value is choosed between <see cref="PlayerRigidBodyMoveManager"/> and
        /// <see cref="PlayerAgentMoveManager"/>. <br/>
        /// Changes can also be made when a specific method is called. For example, if <see cref="SetDestination"/> is called, then the
        /// <see cref="PlayerMoveManagerState"/> will set the FromAgent to true.
        /// /!\ It's value must only be setted from <see cref="EnableFromInput"/> and <see cref="EnableFromAgent"/>
        /// </summary>
        private APlayerMoveManager CurrentPlayerMoveManager;

        private PlayerInteractiveObject PlayerInteractiveObjectRef;

        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();

        #endregion

        public PlayerMoveManager(PlayerInteractiveObject PlayerInteractiveObjectRef,
            PlayerRigidBodyMoveManager PlayerRigidBodyMoveManager, PlayerAgentMoveManager PlayerAgentMoveManager)
        {
            this.PlayerInteractiveObjectRef = PlayerInteractiveObjectRef;
            this.PlayerRigidBodyMoveManager = PlayerRigidBodyMoveManager;
            this.PlayerAgentMoveManager = PlayerAgentMoveManager;
            this.PlayerMoveManagerState = new PlayerMoveManagerState(
                OnFromInputActivated: this.EnableFromInput,
                OnFromAgentActivated: this.EnableFromAgent);
        }

        /// <summary>
        /// Event called from <see cref="PlayerMoveManagerState.FromInput"/> value change.
        /// </summary>
        private void EnableFromInput()
        {
            Debug.Log(MyLog.Format("EnableFromInput"));

            this.PlayerRigidBodyMoveManager.ResetSpeed();
            this.PlayerAgentMoveManager.ResetSpeed();
            this.PlayerInteractiveObjectRef.InteractiveGameObject.Agent.enabled = false;
            this.CurrentPlayerMoveManager = this.PlayerRigidBodyMoveManager;
        }

        /// <summary>
        /// Event called from <see cref="PlayerMoveManagerState.FromAgent"/> value change.
        /// </summary>
        private void EnableFromAgent()
        {
            Debug.Log(MyLog.Format("EnableFromAgent"));

            this.PlayerRigidBodyMoveManager.ResetSpeed();
            this.PlayerAgentMoveManager.ResetSpeed();
            this.PlayerInteractiveObjectRef.InteractiveGameObject.Agent.enabled = true;
            this.CurrentPlayerMoveManager = this.PlayerAgentMoveManager;
        }

        /// <summary>
        /// /!\ Setting a World position destination automatically enable player agent movement <see cref="PlayerMoveManagerState.EnableAgent"/>.
        /// </summary>
        public NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            this.PlayerMoveManagerState.EnableAgent();
            return this.CurrentPlayerMoveManager.SetDestination(IAgentMovementCalculationStrategy);
        }

        public void Tick(float d)
        {
            if (this.GameInputManager.CurrentInput.LocomotionAxis().magnitude >= float.Epsilon)
            {
                this.PlayerMoveManagerState.EnableInput();
            }

            this.CurrentPlayerMoveManager.Tick(d);
        }

        public void AfterTicks()
        {
            this.CurrentPlayerMoveManager.AfterTicks();
        }

        public void FixedTick(float d)
        {
            this.CurrentPlayerMoveManager.FixedTick(d);
        }

        public void ResetSpeed()
        {
            this.CurrentPlayerMoveManager.ResetSpeed();
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.CurrentPlayerMoveManager.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }

        public AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor()
        {
            return this.CurrentPlayerMoveManager.GetCurrentSpeedAttenuationFactor();
        }

        public float GetPlayerSpeedMagnitude()
        {
            return this.CurrentPlayerMoveManager.GetPlayerSpeedMagnitude();
        }

        public void ForceSwitchToAgent()
        {
            this.PlayerMoveManagerState.EnableAgent();
        }
    }

    public class PlayerMoveManagerState
    {
        private BoolVariable FromInput;
        private BoolVariable FromAgent;

        public PlayerMoveManagerState(Action OnFromInputActivated, Action OnFromAgentActivated)
        {
            this.FromInput = new BoolVariable(false, OnFromInputActivated);
            this.FromAgent = new BoolVariable(false, OnFromAgentActivated);
            this.FromInput.SetValue(true);
        }

        public void EnableInput()
        {
            this.FromInput.SetValue(true);
            this.FromAgent.SetValue(false);
        }

        public void EnableAgent()
        {
            this.FromInput.SetValue(false);
            this.FromAgent.SetValue(true);
        }
    }

    public abstract class APlayerMoveManager
    {
        protected ObjectMovementSpeedSystem ObjectMovementSpeedSystem;

        protected APlayerMoveManager(CoreInteractiveObject associatedInteractiveObject, TransformMoveManagerComponentV3 aiTransformMoveManagerComponentV3)
        {
            this.ObjectMovementSpeedSystem = new ObjectMovementSpeedSystem(associatedInteractiveObject, aiTransformMoveManagerComponentV3, AIMovementSpeedAttenuationFactor.RUN, ObjectSpeedCalculationType.MANUAL);
        }

        public virtual void Tick(float d)
        {
        }

        public virtual void AfterTicks()
        {
            this.ObjectMovementSpeedSystem.AfterTicks();
        }

        public virtual void FixedTick(float d)
        {
        }

        public virtual void ResetSpeed()
        {
            this.ObjectMovementSpeedSystem.ResetSpeed();
        }

        public float GetPlayerSpeedMagnitude()
        {
            return this.ObjectMovementSpeedSystem.GetSpeedMagnitude();
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            this.ObjectMovementSpeedSystem.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }

        public virtual NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return NavMeshPathStatus.PathInvalid;
        }

        public AIMovementSpeedAttenuationFactor GetCurrentSpeedAttenuationFactor()
        {
            return this.ObjectMovementSpeedSystem.GetSpeedAttenuationFactor();
        }
    }
}