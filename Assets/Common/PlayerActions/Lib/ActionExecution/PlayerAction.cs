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
        public abstract string PlayerActionUniqueID { get; }
        
        public bool IsAborted { get; private set; }

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

            this.IsAborted = false;

            this.OnPlayerActionStartedCallback = OnPlayerActionStartedCallback;
            this.OnPlayerActionEndCallback = OnPlayerActionEndCallback;
        }

        public virtual bool FinishedCondition()
        {
            return this.IsAborted;
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
            this.OnPlayerActionStartedCallback?.Invoke();
        }

        public void Abort()
        {
            this.Dispose();
            this.IsAborted = true;
        }

        #region Logical Conditions

        public bool CooldownFeatureEnabled()
        {
            return this.CorePlayerActionDefinition.CooldownEnabled;
        }

        public bool MovementAllowed()
        {
            return this.CorePlayerActionDefinition.MovementAllowed;
        }

        #endregion

        #region Data Retrieval

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

        #endregion
    }
}