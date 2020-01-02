using System;
using SelectionWheel;
using UnityEngine;

namespace PlayerActions
{
    /// <summary>
    /// <see cref="PlayerAction"/> are an expansion of the PlayerObject. <br/>
    /// They allow to execute custom logic (with game loop callbacks) aside the PlayerObject own logic. <br/>
    /// Player actions have access to PlayerObject and it's InteractiveObjects methods while PlayerObject has access to <see cref="PlayerActionEntryPoint"/> only. <br/>
    /// Currently executing PlayerAction has full control over the state of the PlayerObject.
    /// </summary>
    public abstract class PlayerAction
    {
        public CorePlayerActionDefinition CorePlayerActionDefinition { get; private set; }

        private float onCooldownTimeElapsed;
        private bool isAborted;

        //-1 is infinite
        private int remainingExecutionAmout;
        private SelectionWheelNodeConfigurationData SelectionWheelNodeConfigurationData;


        #region WorkflowEvents callback

        /// <summary>
        /// Callback called when <see cref="FirstExecution"/> is called.
        /// </summary>
        private Action OnPlayerActionStartedCallback;

        /// <summary>
        /// Callback called when <see cref="Dispose"/> is called.
        /// </summary>
        private Action OnPlayerActionEndCallback;

        #endregion

        protected PlayerAction(CorePlayerActionDefinition CorePlayerActionDefinition, Action OnPlayerActionStartedCallback = null,
            Action OnPlayerActionEndCallback = null)
        {
            var SelectionWheelNodeConfiguration = SelectionWheelNodeConfigurationGameObject.Get().SelectionWheelNodeConfiguration;
            SelectionWheelNodeConfigurationData = SelectionWheelNodeConfiguration.ConfigurationInherentData[CorePlayerActionDefinition.ActionWheelNodeConfigurationId];

            this.CorePlayerActionDefinition = CorePlayerActionDefinition;

            //on init, it is available
            onCooldownTimeElapsed = this.CorePlayerActionDefinition.CorePlayerActionCooldownDefinition.CoolDownTime * 2;
            remainingExecutionAmout = this.CorePlayerActionDefinition.ExecutionAmount;
            this.isAborted = false;

            this.OnPlayerActionStartedCallback = OnPlayerActionStartedCallback;
            this.OnPlayerActionEndCallback = OnPlayerActionEndCallback;
        }

        public virtual bool FinishedCondition()
        {
            return this.isAborted;
        }

        public virtual void FixedTick(float d)
        {
        }

        public abstract void Tick(float d);
        public abstract void AfterTicks(float d);
        public abstract void TickTimeFrozen(float d);
        public abstract void LateTick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

        public virtual void Dispose()
        {
            this.OnPlayerActionEndCallback?.Invoke();
        }

        public virtual void FirstExecution()
        {
            this.PlayerActionConsumed();
            this.OnPlayerActionStartedCallback?.Invoke();
        }

        public void Abort()
        {
            this.Dispose();
            this.isAborted = true;
        }

        public void CoolDownTick(float d)
        {
            onCooldownTimeElapsed += d;
        }

        private void PlayerActionConsumed()
        {
            onCooldownTimeElapsed = 0f;
            if (remainingExecutionAmout > 0) remainingExecutionAmout -= 1;
        }

        public void IncreaseActionRemainingExecutionAmount(int deltaIncrease)
        {
            remainingExecutionAmout += deltaIncrease;
        }

        #region Logical Conditions

        public bool CooldownFeatureEnabled()
        {
            return this.CorePlayerActionDefinition.CooldownEnabled;
        }
        
        public bool IsOnCoolDown()
        {
            return this.CooldownFeatureEnabled() && onCooldownTimeElapsed < this.CorePlayerActionDefinition.CorePlayerActionCooldownDefinition.CoolDownTime;
        }

        public bool CanBeExecuted()
        {
            return !IsOnCoolDown() && HasStillSomeExecutionAmount();
        }

        public bool HasStillSomeExecutionAmount()
        {
            return remainingExecutionAmout != 0;
        }

        public bool MovementAllowed()
        {
            return this.CorePlayerActionDefinition.MovementAllowed;
        }

        #endregion

        #region Data Retrieval

        public float GetCooldownRemainingTime()
        {
            return this.CorePlayerActionDefinition.CorePlayerActionCooldownDefinition.CoolDownTime - onCooldownTimeElapsed;
        }

        public SelectionWheelNodeConfigurationId GetSelectionWheelConfigurationId()
        {
            return CorePlayerActionDefinition.ActionWheelNodeConfigurationId;
        }

        public string GetDescriptionText()
        {
            return SelectionWheelNodeConfigurationData.DescriptionText;
        }

        public Sprite GetNodeIcon()
        {
            return SelectionWheelNodeConfigurationData.WheelNodeIcon;
        }

        public int RemainingExecutionAmout => remainingExecutionAmout;

        #endregion
    }
}