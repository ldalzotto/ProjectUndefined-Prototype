using System;
using System.Collections;
using System.Collections.Generic;
using InteractiveObjects;
using UnityEngine.Profiling;

namespace PlayerActions
{
    public class PlayerActionPlayerSystem
    {
        private PlayerActionExecutionManager PlayerActionExecutionManager;


        public PlayerActionPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            PlayerActionExecutionManager = new PlayerActionExecutionManager();
            AssociatedInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnAssociatedInteractiveObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : FixedTick");
            PlayerActionExecutionManager.FixedTick(d);
            Profiler.EndSample();
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : Tick");
            PlayerActionExecutionManager.Tick(d);
            Profiler.EndSample();
        }

        public void AfterTicks(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : AfterTicks");
            PlayerActionExecutionManager.AfterTicks(d);
            Profiler.EndSample();
        }

        public void TickTimeFrozen(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : TickTimeFrozen");
            PlayerActionExecutionManager.TickTimeFrozen(d);
            Profiler.EndSample();
        }

        public void LateTick(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : LateTick");
            PlayerActionExecutionManager.LateTick(d);
            Profiler.EndSample();
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
            Profiler.BeginSample("PlayerActionPlayerSystem : ExecuteAction");
            PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
            Profiler.EndSample();
        }

        #region External Events

        private void OnAssociatedInteractiveObjectDestroyed(CoreInteractiveObject DestroyedInteractiveObject)
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

    /// <summary>
    /// Execute concurrently <see cref="PlayerAction"/> ordered by their types (<see cref="currentlyPlayingActions"/>).
    /// When a <see cref="PlayerAction"/> is on cooldown (state tracked by <see cref="ActionsCooldownStates"/>), the action is not executed.
    /// </summary>
    class PlayerActionExecutionManager
    {
        private Dictionary<Type, PlayerAction> currentlyPlayingActions = new Dictionary<Type, PlayerAction>();
        private Stack<KeyValuePair<Type, PlayerAction>> stoppedPlayerActionBuffer = new Stack<KeyValuePair<Type, PlayerAction>>();

        private Dictionary<Type, CooldownActionState> ActionsCooldownStates = new Dictionary<Type, CooldownActionState>();
        private Stack<KeyValuePair<Type, CooldownActionState>> ActionsCooldownStatesUpdateBuffer = new Stack<KeyValuePair<Type, CooldownActionState>>();
        private Stack<Type> CooldownEndedBuffer = new Stack<Type>();


        private BoolVariable CurrentlyPlayerActionsLocked;
        private Stack<PlayerAction> LockedBufferPlayerActionsExecuted = new Stack<PlayerAction>();

        public PlayerActionExecutionManager()
        {
            this.CurrentlyPlayerActionsLocked = new BoolVariable(false, onJustSetToFalse: this.OnCurrentlyPlayingActionsUnlocked);
        }

        public void ExecuteAction(PlayerAction PlayerAction)
        {
            if (this.CurrentlyPlayerActionsLocked.GetValue())
            {
                this.LockedBufferPlayerActionsExecuted.Push(PlayerAction);
            }
            else
            {
                this.LockCurrentlyPlayingActions();


                var playerActionType = PlayerAction.GetType();
                if (this.IsActionOfTypeAllowedToBePlaying(playerActionType))
                {
                    if (this.currentlyPlayingActions.ContainsKey(playerActionType))
                    {
                        this.StopAction(this.currentlyPlayingActions[playerActionType]);
                    }

                    this.currentlyPlayingActions[playerActionType] = PlayerAction;
                    this.ActionsCooldownStates[playerActionType] = new CooldownActionState(PlayerAction.CorePlayerActionDefinition);

                    PlayerAction.FirstExecution();
                }

                this.UnLockCurrentlyPlayingActions();
            }
        }

        public void FixedTick(float d)
        {
            this.LockCurrentlyPlayingActions();

            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.FixedTick(d);

            this.UnLockCurrentlyPlayingActions();

            this.DiscardFinishedSkillActions();
        }

        public void Tick(float d)
        {
            this.LockCurrentlyPlayingActions();

            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.Tick(d);

            this.UnLockCurrentlyPlayingActions();

            this.DiscardFinishedSkillActions();
            this.UpdateCooldowns(d);
        }

        public void AfterTicks(float d)
        {
            this.LockCurrentlyPlayingActions();

            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.AfterTicks(d);

            this.UnLockCurrentlyPlayingActions();
            this.DiscardFinishedSkillActions();
        }

        public void TickTimeFrozen(float d)
        {
            this.LockCurrentlyPlayingActions();

            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.TickTimeFrozen(d);

            this.UnLockCurrentlyPlayingActions();
            this.DiscardFinishedSkillActions();
        }

        public void LateTick(float d)
        {
            this.LockCurrentlyPlayingActions();

            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.LateTick(d);

            this.UnLockCurrentlyPlayingActions();
            this.DiscardFinishedSkillActions();
        }

        private void DiscardFinishedSkillActions()
        {
            foreach (var currentlyPlayingActionEntry in this.currentlyPlayingActions)
            {
                if (currentlyPlayingActionEntry.Value.FinishedCondition())
                {
                    this.stoppedPlayerActionBuffer.Push(currentlyPlayingActionEntry);
                }
            }

            while (this.stoppedPlayerActionBuffer.Count > 0)
            {
                this.StopAction(this.stoppedPlayerActionBuffer.Pop().Value);
            }
        }

        private void UpdateCooldowns(float d)
        {
            foreach (var cooldownActionState in this.ActionsCooldownStates)
            {
                var cooldownState = cooldownActionState.Value;
                cooldownState.Tick(d);
                this.ActionsCooldownStatesUpdateBuffer.Push(new KeyValuePair<Type, CooldownActionState>(cooldownActionState.Key, cooldownState));

                if (!cooldownState.IsOnCooldown())
                {
                    this.CooldownEndedBuffer.Push(cooldownActionState.Key);
                }
            }

            while (this.ActionsCooldownStatesUpdateBuffer.Count > 0)
            {
                var actionCooldownStateEntry = this.ActionsCooldownStatesUpdateBuffer.Pop();
                this.ActionsCooldownStates[actionCooldownStateEntry.Key] = actionCooldownStateEntry.Value;
            }


            while (this.CooldownEndedBuffer.Count > 0)
            {
                this.ActionsCooldownStates.Remove(this.CooldownEndedBuffer.Pop());
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

        private void LockCurrentlyPlayingActions()
        {
            this.CurrentlyPlayerActionsLocked.SetValue(true);
        }

        private void UnLockCurrentlyPlayingActions()
        {
            this.CurrentlyPlayerActionsLocked.SetValue(false);
        }

        private void OnCurrentlyPlayingActionsUnlocked()
        {
            while (this.LockedBufferPlayerActionsExecuted.Count > 0)
            {
                this.ExecuteAction(this.LockedBufferPlayerActionsExecuted.Pop());
            }
        }

        public void StopAllActions()
        {
            foreach (var playingPlayerAction in currentlyPlayingActions.Values)
                playingPlayerAction.Dispose();

            this.currentlyPlayingActions.Clear();
            this.ActionsCooldownStates.Clear();
        }

        #region Logical Conditions

        public bool DoesActionOfTypeIsPlaying(Type actionType)
        {
            return this.currentlyPlayingActions.ContainsKey(actionType);
        }

        public bool IsActionOfTypeAllowedToBePlaying(Type actionType)
        {
            return !this.DoesActionOfTypeIsPlaying(actionType) && (!this.ActionsCooldownStates.ContainsKey(actionType) || !this.ActionsCooldownStates[actionType].IsOnCooldown());
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

    struct CooldownActionState
    {
        public bool CooldownFeatureEnabled;

        private float TargetCooldownTime;
        private float CurrentTimeElapsed;

        public CooldownActionState(CorePlayerActionDefinition CorePlayerActionDefinition)
        {
            this.CooldownFeatureEnabled = CorePlayerActionDefinition.CooldownEnabled;
            this.TargetCooldownTime = 0f;

            if (this.CooldownFeatureEnabled)
            {
                TargetCooldownTime = CorePlayerActionDefinition.CorePlayerActionCooldownDefinition.CoolDownTime;
            }

            CurrentTimeElapsed = 0f;
        }

        public void Tick(float d)
        {
            this.CurrentTimeElapsed += d;
        }

        public bool IsOnCooldown()
        {
            return this.CooldownFeatureEnabled && (this.TargetCooldownTime > 0f && this.CurrentTimeElapsed <= this.TargetCooldownTime);
        }
    }

    #endregion
}