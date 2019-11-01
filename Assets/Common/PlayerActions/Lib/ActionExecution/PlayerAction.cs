using SelectionWheel;
using UnityEngine;

namespace PlayerActions
{
    public abstract class PlayerAction
    {
        private CooldownEventTrackerManager CooldownEventTrackerManager;

        protected CorePlayerActionDefinition CorePlayerActionDefinition;

        private float onCooldownTimeElapsed;

        public PlayerActionType PlayerActionType;

        //-1 is infinite
        private int remainingExecutionAmout;
        private SelectionWheelNodeConfigurationData SelectionWheelNodeConfigurationData;


        protected PlayerAction(CorePlayerActionDefinition CorePlayerActionDefinition)
        {
            PlayerActionType = CorePlayerActionDefinition.PlayerActionType;
            var SelectionWheelNodeConfiguration = SelectionWheelNodeConfigurationGameObject.Get().SelectionWheelNodeConfiguration;
            SelectionWheelNodeConfigurationData = SelectionWheelNodeConfiguration.ConfigurationInherentData[CorePlayerActionDefinition.ActionWheelNodeConfigurationId];

            this.CorePlayerActionDefinition = CorePlayerActionDefinition;

            //on init, it is available
            onCooldownTimeElapsed = this.CorePlayerActionDefinition.CoolDownTime * 2;
            remainingExecutionAmout = this.CorePlayerActionDefinition.ExecutionAmount;
        }

        public abstract bool FinishedCondition();
        public abstract void Tick(float d);
        public abstract void LateTick(float d);
        public abstract void GUITick();
        public abstract void GizmoTick();

        public virtual void FirstExecution()
        {
            CooldownEventTrackerManager = new CooldownEventTrackerManager();
        }

        public void CoolDownTick(float d)
        {
            onCooldownTimeElapsed += d;
            if (!IsOnCoolDown()) CooldownEventTrackerManager.Tick(this);
        }

        protected void PlayerActionConsumed()
        {
            onCooldownTimeElapsed = 0f;
            CooldownEventTrackerManager.ResetCoolDown();
            if (remainingExecutionAmout > 0) remainingExecutionAmout -= 1;
        }

        public void IncreaseActionRemainingExecutionAmount(int deltaIncrease)
        {
            remainingExecutionAmout += deltaIncrease;
        }

        #region Logical Conditions

        public bool IsOnCoolDown()
        {
            return onCooldownTimeElapsed < CorePlayerActionDefinition.CoolDownTime;
        }

        public bool CanBeExecuted()
        {
            return !IsOnCoolDown() && HasStillSomeExecutionAmount();
        }

        public bool HasStillSomeExecutionAmount()
        {
            return remainingExecutionAmout != 0;
        }

        #endregion

        #region Data Retrieval

        public float GetCooldownRemainingTime()
        {
            return CorePlayerActionDefinition.CoolDownTime - onCooldownTimeElapsed;
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

    #region Cooldown Tracking

    internal class CooldownEventTrackerManager
    {
        private bool endOfCooldownEventEmitted;

        public CooldownEventTrackerManager()
        {
            endOfCooldownEventEmitted = false;
        }

        public void Tick(PlayerAction involvedAction)
        {
            if (!endOfCooldownEventEmitted) endOfCooldownEventEmitted = true;
        }

        public void ResetCoolDown()
        {
            endOfCooldownEventEmitted = false;
        }
    }

    #endregion
}