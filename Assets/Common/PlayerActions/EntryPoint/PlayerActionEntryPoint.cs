using System.Collections.Generic;
using CoreGame;
using SelectableObjects_Interfaces;
using UnityEngine;

namespace PlayerActions
{
    public class PlayerActionEntryPoint : GameSingleton<PlayerActionEntryPoint>
    {
        private PlayerActionManager PlayerActionManager = PlayerActionManager.Get();
        private PlayerActionWheelManager PlayerActionWheelManager = PlayerActionWheelManager.Get();

        public void Init()
        {
            #region Event Register

            SelectableObjectEventsManager.Get().RegisterOnSelectableObjectSelectedEventAction(this.OnSelectableObjectSelected);
            SelectableObjectEventsManager.Get().RegisterOnSelectableObjectNoMoreSelectedEventAction(this.OnSelectableObjectDeSelected);

            #endregion

            PlayerActionManager.Init();
            PlayerActionWheelManager.Init();
        }

        #region Per Frame Methods

        public void Tick(float d)
        {
            this.PlayerActionManager.Tick(d);
            this.PlayerActionWheelManager.Tick(d);
        }

        public void LateTick(float d)
        {
            this.PlayerActionManager.LateTick(d);
            this.PlayerActionWheelManager.LateTick(d);
        }

        public void GizmoTick()
        {
            this.PlayerActionManager.GizmoTick();
        }

        public void GUITick()
        {
            this.PlayerActionManager.GUITick();
        }

        #endregion

        #region Logical Conditions

        public bool IsActionExecuting()
        {
            return this.PlayerActionManager.IsActionExecuting();
        }

        public bool IsSelectionWheelEnabled()
        {
            return this.PlayerActionWheelManager.IsSelectionWheelEnabled();
        }

        #endregion

        #region Data Retrieval

        public RTPPlayerAction GetCurrentlySelectedPlayerAction()
        {
            return this.PlayerActionWheelManager.GetCurrentlySelectedPlayerAction();
        }

        #endregion

        #region Events and methods

        public void IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction RTPPlayerAction, int deltaRemaining)
        {
            this.PlayerActionManager.IncreaseOrAddActionsRemainingExecutionAmount(RTPPlayerAction, deltaRemaining);
        }

        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            this.PlayerActionManager.ExecuteAction(rTPPlayerAction);
            this.PlayerActionWheelManager.PlayerActionWheelSleep(false);
        }

        public void AwakePlayerActionSelectionWheel(Transform followingWorldTransform)
        {
            this.PlayerActionWheelManager.PlayerActionWheelAwake(this.PlayerActionManager.GetCurrentAvailablePlayerActions(), followingWorldTransform);
        }

        public void SleepPlayerActionSelectionWheel(bool destroyImmediate)
        {
            this.PlayerActionWheelManager.PlayerActionWheelSleep(destroyImmediate);
        }

        public void AddActionsToAvailable(List<RTPPlayerAction> addedActions)
        {
            this.PlayerActionManager.AddActionsToAvailable(addedActions);
        }

        public void RemoveActionsToAvailable(List<RTPPlayerAction> removedActions)
        {
            this.PlayerActionManager.RemoveActionsToAvailable(removedActions);
        }

        private void OnSelectableObjectSelected(ISelectableObjectSystem SelectableObject)
        {
            this.PlayerActionManager.AddActionToAvailable(SelectableObject.AssociatedPlayerAction as RTPPlayerAction);
            this.PlayerActionWheelManager.PlayerActionWheelRefresh(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }

        private void OnSelectableObjectDeSelected(ISelectableObjectSystem SelectableObject)
        {
            this.PlayerActionManager.RemoveActionToAvailable(SelectableObject.AssociatedPlayerAction as RTPPlayerAction);
            this.PlayerActionWheelManager.PlayerActionWheelRefresh(this.PlayerActionManager.GetCurrentAvailablePlayerActions());
        }

        #endregion
    }
}