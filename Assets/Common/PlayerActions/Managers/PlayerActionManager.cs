using System;
using System.Collections.Generic;
using CoreGame;
using PlayerObject_Interfaces;
using SelectionWheel;
using UnityEngine;

namespace PlayerActions
{
    public class PlayerActionManager : GameSingleton<PlayerActionManager>
    {
        private PlayerActionExecutionManager PlayerActionExecutionManager;
        private PlayerActionsAvailableManager PlayerActionsAvailableManager;

        #region Internal Events

        private event Action OnPlayerActionFinishedEvent;

        #endregion

        public void Init()
        {
            #region Event Register

            OnPlayerActionFinishedEvent += OnPlayerActionFinished;

            #endregion

            PlayerActionExecutionManager = new PlayerActionExecutionManager(() => OnPlayerActionFinishedEvent.Invoke());
            PlayerActionsAvailableManager = new PlayerActionsAvailableManager();
            PlayerInteractiveObjectDestroyedEvent.Get().RegisterPlayerInteractiveObjectDestroyedEvent(this.OnPlayerObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            PlayerActionExecutionManager.FixedTick(d);
        }
        
        public void Tick(float d)
        {
            PlayerActionExecutionManager.Tick(d);
        }

        public void LateTick(float d)
        {
            PlayerActionExecutionManager.LateTick(d);
        }

        public void GizmoTick()
        {
            PlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            PlayerActionExecutionManager.GUITick();
        }

        internal void ExecuteAction(PlayerAction rTPPlayerAction)
        {
            PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
        }

        private void OnPlayerActionFinished()
        {
            PlayerActionExecutionManager.StopAction();
        }

        internal void IncreaseOrAddActionsRemainingExecutionAmount(PlayerAction playerAction, int deltaRemaining)
        {
            PlayerActionsAvailableManager.IncreaseOrAddActionsRemainingExecutionAmount(playerAction, deltaRemaining);
        }

        internal void AddActionsToAvailable(List<PlayerAction> addedActions)
        {
            foreach (var addedAction in addedActions) AddActionToAvailable(addedAction);
        }

        internal void RemoveActionsToAvailable(List<PlayerAction> removedActions)
        {
            foreach (var removedAction in removedActions) this.PlayerActionsAvailableManager.RemoveActionToAvailable(removedAction);
        }

        internal void RemoveActionToAvailable(PlayerAction removedAction)
        {
            this.PlayerActionsAvailableManager.RemoveActionToAvailable(removedAction);
        }

        internal void AddActionToAvailable(PlayerAction addedAction)
        {
            PlayerActionsAvailableManager.AddActionToAvailable(addedAction);
        }

        #region External Events

        private void OnPlayerObjectDestroyed()
        {
            this.PlayerActionExecutionManager.StopAction();
        }

        #endregion

        #region Logical Conditions

        public bool IsActionExecuting()
        {
            return PlayerActionExecutionManager.IsActionExecuting;
        }

        public bool DoesCurrentActionAllowsMovement()
        {
            return this.PlayerActionExecutionManager.CurrentAction != null &&
                   this.PlayerActionExecutionManager.CurrentAction.MovementAllowed();
        }
        
        #endregion

        #region Data Retrieval

        internal List<PlayerAction> GetCurrentAvailablePlayerActions()
        {
            return this.PlayerActionsAvailableManager.CurrentAvailableActions.MultiValueGetValues();
        }

        public PlayerAction GetCurrentlyPlayingPlayerAction()
        {
            return this.PlayerActionExecutionManager.CurrentAction;
        }

        #endregion

    }


    #region Action execution

    internal class PlayerActionExecutionManager
    {
        private PlayerAction currentAction;
        private bool isActionExecuting;
        private Action TriggerOnPlayerActionFinichedEventAction;

        public PlayerActionExecutionManager(Action TriggerOnPlayerActionFinichedEventAction)
        {
            this.TriggerOnPlayerActionFinichedEventAction = TriggerOnPlayerActionFinichedEventAction;
        }

        public bool IsActionExecuting => isActionExecuting;

        public PlayerAction CurrentAction => currentAction;

        public void FixedTick(float d)
        {
            if (this.CurrentAction != null)
            {
                if (currentAction.FinishedCondition())
                    TriggerOnPlayerActionFinichedEventAction.Invoke();
                else
                    currentAction.FixedTick(d);
            }
        }
        
        public void Tick(float d)
        {
            if (currentAction != null)
            {
                if (currentAction.FinishedCondition())
                    TriggerOnPlayerActionFinichedEventAction.Invoke();
                else
                    currentAction.Tick(d);
            }
        }

        public void LateTick(float d)
        {
            if (currentAction != null) currentAction.LateTick(d);
        }

        public void GizmoTick()
        {
            if (currentAction != null) currentAction.GizmoTick();
        }

        public void GUITick()
        {
            if (currentAction != null) currentAction.GUITick();
        }

        public void ExecuteAction(PlayerAction PlayerAction)
        {
            currentAction = PlayerAction;
            isActionExecuting = true;
            currentAction.FirstExecution();
        }

        public void StopAction()
        {
            if (this.CurrentAction != null)
            {
                this.CurrentAction.Dispose();
                this.currentAction = null;
            }

            isActionExecuting = false;
        }
    }

    #endregion

    #region RTPPlayer actions availability

    internal class PlayerActionsAvailableManager
    {
        private MultiValueDictionary<PlayerActionType, PlayerAction> currentAvailableActions;

        public PlayerActionsAvailableManager()
        {
            currentAvailableActions = new MultiValueDictionary<PlayerActionType, PlayerAction>();
        }

        public MultiValueDictionary<PlayerActionType, PlayerAction> CurrentAvailableActions => currentAvailableActions;

        public void Tick(float d, float timeAttenuation)
        {
            foreach (var availableAction in currentAvailableActions.MultiValueGetValues())
                if (availableAction.IsOnCoolDown())
                    availableAction.CoolDownTick(d * timeAttenuation);
        }

        public void AddActionToAvailable(PlayerAction rTPPlayerActionToAdd)
        {
            currentAvailableActions.MultiValueAdd(rTPPlayerActionToAdd.PlayerActionType, rTPPlayerActionToAdd);
        }

        public void RemoveActionToAvailable(PlayerAction rTPPlayerActionToRemove)
        {
            currentAvailableActions.MultiValueRemove(rTPPlayerActionToRemove.PlayerActionType, rTPPlayerActionToRemove);
        }

        public void IncreaseOrAddActionsRemainingExecutionAmount(PlayerAction playerAction, int deltaRemaining)
        {
            if (playerAction.PlayerActionType != PlayerActionType.UNCLASSIFIED)
            {
                currentAvailableActions.TryGetValue(playerAction.PlayerActionType, out var retrievedActions);
                if (retrievedActions != null && retrievedActions.Count > 0)
                    foreach (var action in retrievedActions)
                        action.IncreaseActionRemainingExecutionAmount(deltaRemaining);
                else //Wa add
                    currentAvailableActions.MultiValueAdd(playerAction.PlayerActionType, playerAction);
            }
            else //Wa add
            {
                currentAvailableActions.MultiValueAdd(playerAction.PlayerActionType, playerAction);
            }
        }
    }

    #endregion

    #region RTPPlayer action wheel node data

    public class PlayerSelectionWheelNodeData : SelectionWheelNodeData
    {
        private PlayerAction playerAction;

        public PlayerSelectionWheelNodeData(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override object Data => playerAction;

        public override bool IsOnCoolDown => playerAction.IsOnCoolDown();

        public override float GetRemainingCooldownTime => playerAction.GetCooldownRemainingTime();

        public override int GetRemainingExecutionAmount => playerAction.RemainingExecutionAmout;

        public override bool CanNodeBeExecuted => playerAction.CanBeExecuted();

        public override string NodeText => playerAction.GetDescriptionText();

        public override Sprite NodeSprite => playerAction.GetNodeIcon();
    }

    #endregion
}