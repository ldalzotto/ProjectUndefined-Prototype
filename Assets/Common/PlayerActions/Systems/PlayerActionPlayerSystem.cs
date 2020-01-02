using System;
using System.Collections.Generic;
using CoreGame;
using PlayerObject_Interfaces;
using SelectionWheel;
using UnityEngine;

namespace PlayerActions
{
    public class PlayerActionPlayerSystem
    {
        private PlayerActionExecutionManager PlayerActionExecutionManager;


        public PlayerActionPlayerSystem()
        {
            PlayerActionExecutionManager = new PlayerActionExecutionManager();
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

        public void AfterTicks(float d)
        {
            PlayerActionExecutionManager.AfterTicks(d);
        }

        public void TickTimeFrozen(float d)
        {
            PlayerActionExecutionManager.TickTimeFrozen(d);
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

        public void ExecuteAction(PlayerAction rTPPlayerAction)
        {
            PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
        }

        #region External Events

        private void OnPlayerObjectDestroyed()
        {
            this.PlayerActionExecutionManager.StopAllActions();
        }

        #endregion

        #region Logical Conditions

        public bool DoesActionOfTypeIsPlaying(Type actionType)
        {
            return PlayerActionExecutionManager.DoesActionOfTypeIsPlaying(actionType);
        }

        public bool IsActionOfTypeAllowedToBePlaying(Type actionType)
        {
            return PlayerActionExecutionManager.IsActionOfTypeAllowedToBePlaying(actionType);
        }

        public bool DoesCurrentActionAllowsMovement()
        {
            return this.PlayerActionExecutionManager.DoesActionsAllowMovement();
        }

        #endregion
    }


    #region Action execution

    internal class PlayerActionExecutionManager
    {
        private Dictionary<Type, PlayerAction> currentlyPlayingActions = new Dictionary<Type, PlayerAction>();
        private Dictionary<Type, PlayerAction> onCooldownActions = new Dictionary<Type, PlayerAction>();

        public PlayerActionExecutionManager()
        {
        }

        public void ExecuteAction(PlayerAction PlayerAction)
        {
            var playerActionType = PlayerAction.GetType();
            if (!this.onCooldownActions.ContainsKey(playerActionType))
            {
                if (this.currentlyPlayingActions.ContainsKey(playerActionType))
                {
                    this.StopAction(this.currentlyPlayingActions[playerActionType]);
                }

                this.currentlyPlayingActions[playerActionType] = PlayerAction;

                if (PlayerAction.CooldownFeatureEnabled())
                {
                    this.onCooldownActions[playerActionType] = PlayerAction;
                }

                PlayerAction.FirstExecution();
            }
        }

        public void FixedTick(float d)
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.FixedTick(d);

            this.DiscardFinishedSkillActions();
        }

        public void Tick(float d)
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.Tick(d);

            this.DiscardFinishedSkillActions();
            this.UpdateCooldowns(d);
        }

        public void AfterTicks(float d)
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.AfterTicks(d);

            this.DiscardFinishedSkillActions();
        }

        public void TickTimeFrozen(float d)
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.TickTimeFrozen(d);

            this.DiscardFinishedSkillActions();
        }

        public void LateTick(float d)
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.LateTick(d);

            this.DiscardFinishedSkillActions();
        }

        private void DiscardFinishedSkillActions()
        {
            List<PlayerAction> finishedPlayerActions = null;
            foreach (var playerAction in this.currentlyPlayingActions.Values)
            {
                if (playerAction.FinishedCondition())
                {
                    if (finishedPlayerActions == null)
                    {
                        finishedPlayerActions = new List<PlayerAction>();
                    }

                    finishedPlayerActions.Add(playerAction);
                }
            }

            if (finishedPlayerActions != null)
            {
                foreach (var finishedPlayerAction in finishedPlayerActions)
                {
                    this.StopAction(finishedPlayerAction);
                }
            }
        }

        private void UpdateCooldowns(float d)
        {
            List<PlayerAction> CooldownEndedPlayerActions = null;

            foreach (var playingPlayerAction in onCooldownActions.Values)
            {
                playingPlayerAction.CoolDownTick(d);
                if (!playingPlayerAction.IsOnCoolDown())
                {
                    if (CooldownEndedPlayerActions == null)
                    {
                        CooldownEndedPlayerActions = new List<PlayerAction>();
                    }

                    CooldownEndedPlayerActions.Add(playingPlayerAction);
                }
            }


            if (CooldownEndedPlayerActions != null)
            {
                foreach (var type in CooldownEndedPlayerActions)
                {
                    this.onCooldownActions.Remove(type.GetType());
                }
            }
        }

        public void GizmoTick()
        {
            //  if (currentAction != null) currentAction.GizmoTick();
        }

        public void GUITick()
        {
            //   if (currentAction != null) currentAction.GUITick();
        }

        private void StopAction(PlayerAction playerAction)
        {
            playerAction.Dispose();
            this.currentlyPlayingActions.Remove(playerAction.GetType());
        }

        public void StopAllActions()
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.Dispose();

            foreach (var onCooldownPlayerAction in onCooldownActions.Values)
                onCooldownPlayerAction.Dispose();

            this.currentlyPlayingActions.Clear();
            this.onCooldownActions.Clear();
        }

        #region Logical Conditions

        public bool DoesActionOfTypeIsPlaying(Type actionType)
        {
            return this.currentlyPlayingActions.ContainsKey(actionType);
        }

        public bool IsActionOfTypeAllowedToBePlaying(Type actionType)
        {
            return !this.DoesActionOfTypeIsPlaying(actionType) && !this.onCooldownActions.ContainsKey(actionType);
        }

        public bool DoesActionsAllowMovement()
        {
            foreach (var value in this.currentlyPlayingActions.Values)
            {
                if (!value.CorePlayerActionDefinition.MovementAllowed)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }

    #endregion

    /*
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
    */
}