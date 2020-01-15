using System;
using CoreGame;
using Input;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace PlayerObject
{
    /// <summary>
    /// Manages <see cref="PlayerAimingInteractiveObject"/> movement from Input <see cref="PlayerRigidBodyMoveManager"/> and from Agent <see cref="PlayerAgentMoveManager"/>.
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

        private PlayerInteractiveObject _playerIeractiveObjectRef;
        private ObjectMovementSpeedSystem PlayerObjectMovementSpeedSystemRef;

        #region External Dependencies

        private GameInputManager GameInputManager = GameInputManager.Get();

        #endregion

        public PlayerMoveManager(PlayerInteractiveObject playerAimingInteractiveObjectRef, ObjectMovementSpeedSystem PlayerObjectMovementSpeedSystemRef,
            PlayerRigidBodyMoveManager PlayerRigidBodyMoveManager, PlayerAgentMoveManager PlayerAgentMoveManager)
        {
            this._playerIeractiveObjectRef = playerAimingInteractiveObjectRef;
            this.PlayerObjectMovementSpeedSystemRef = PlayerObjectMovementSpeedSystemRef;
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

            this.PlayerObjectMovementSpeedSystemRef.ResetSpeed();
            this.PlayerObjectMovementSpeedSystemRef.SetObjectSpeedCalculationType(ObjectSpeedCalculationType.MANUAL);
            this.PlayerRigidBodyMoveManager.ResetSpeed();
            this.PlayerAgentMoveManager.ResetSpeed();
            this._playerIeractiveObjectRef.InteractiveGameObject.Agent.enabled = false;
            this.CurrentPlayerMoveManager = this.PlayerRigidBodyMoveManager;
        }

        /// <summary>
        /// Event called from <see cref="PlayerMoveManagerState.FromAgent"/> value change.
        /// </summary>
        private void EnableFromAgent()
        {
            Debug.Log(MyLog.Format("EnableFromAgent"));

            this.PlayerObjectMovementSpeedSystemRef.ResetSpeed();
            this.PlayerObjectMovementSpeedSystemRef.SetObjectSpeedCalculationType(ObjectSpeedCalculationType.AGENT);
            this.PlayerRigidBodyMoveManager.ResetSpeed();
            this.PlayerAgentMoveManager.ResetSpeed();
            this._playerIeractiveObjectRef.InteractiveGameObject.Agent.enabled = true;
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

        public void AfterTicks(float d)
        {
            this.CurrentPlayerMoveManager.AfterTicks(d);
        }

        public void FixedTick(float d)
        {
            this.CurrentPlayerMoveManager.FixedTick(d);
        }

        public void LateTick(float d)
        {
            this.CurrentPlayerMoveManager.LateTick(d);
        }

        public void ResetSpeed()
        {
            this.CurrentPlayerMoveManager.ResetSpeed();
        }

        public void ForceSwitchToAgent()
        {
            this.PlayerMoveManagerState.EnableAgent();
        }
        
        public void SetConstraintForThisFrame(PlayerMovementConstraint PlayerMovementConstraint)
        {
            this.CurrentPlayerMoveManager.SetConstraintForThisFrame(PlayerMovementConstraint);
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
        /// <summary>
        /// The current player movement constraint <see cref="PlayerMovementConstraint"/>.
        /// /!\ <see cref="CurrentConstraint"/> is consumed every frame. This means that for a constraint to be applied in an interval of time,
        /// <see cref="SetConstraintForThisFrame"/> must be called every frame.
        /// This value is setted via <see cref="SetConstraintForThisFrame"/>.
        /// </summary>
        protected PlayerMovementConstraint CurrentConstraint;

        public virtual void Tick(float d)
        {
        }

        public virtual void AfterTicks(float d)
        {
        }

        public virtual void FixedTick(float d)
        {
        }
        
        public virtual void LateTick(float d){}

        public virtual void ResetSpeed()
        {
        }

        public void SetConstraintForThisFrame(PlayerMovementConstraint PlayerMovementConstraint)
        {
            this.CurrentConstraint = PlayerMovementConstraint;
        }
        
        public virtual NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return NavMeshPathStatus.PathInvalid;
        }
    }
    
}