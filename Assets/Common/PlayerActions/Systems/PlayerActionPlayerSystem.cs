using System;
using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace PlayerActions
{
    public class PlayerActionPlayerSystem
    {
        private PlayerActionExecutionManagerV2 PlayerActionExecutionManagerV2;
        private CoreInteractiveObject AssociatedInteractiveObject;

        public PlayerActionPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.PlayerActionExecutionManagerV2 = new PlayerActionExecutionManagerV2();
            this.PlayerActionExecutionManagerV2.Init();

            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            AssociatedInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnAssociatedInteractiveObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            this.PlayerActionExecutionManagerV2.FixedTick(d);
        }

        public void Tick(float d)
        {
            this.PlayerActionExecutionManagerV2.Tick(d);
        }

        public void AfterTicks(float d)
        {
            this.PlayerActionExecutionManagerV2.AfterTicks(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.PlayerActionExecutionManagerV2.TickTimeFrozen(d);
        }

        public void LateTick(float d)
        {
            this.PlayerActionExecutionManagerV2.LateTick(d);
        }

        public void GizmoTick()
        {
            // PlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            //  PlayerActionExecutionManager.GUITick();
        }

        public void ExecuteActionV2(PlayerActionInherentData PlayerActionInherentData, Action OnPlayerActionStartedCallback = null, Action OnPlayerActionEndCallback = null)
        {
            if (this.IsActionOfTypeAllowedToBePlaying(PlayerActionInherentData.PlayerActionUniqueID))
            {
                var playerActionInput = PlayerActionInherentData.BuildInputFromInteractiveObject(this.AssociatedInteractiveObject);
                if (playerActionInput != null)
                {
                    this.PlayerActionExecutionManagerV2.Execute(PlayerActionInherentData.BuildPlayerAction(playerActionInput, OnPlayerActionStartedCallback, OnPlayerActionEndCallback));
                }
            }
        }

        #region External Events

        private void OnAssociatedInteractiveObjectDestroyed(CoreInteractiveObject DestroyedInteractiveObject)
        {
            this.PlayerActionExecutionManagerV2.StopAllActions();
        }

        #endregion

        #region Logical Conditions

        public bool IsActionOfTypeIsAlreadyPlaying(string actionUniqueID)
        {
            return this.PlayerActionExecutionManagerV2.DoesActionOfTypeIsPlaying(actionUniqueID);
        }
        
        public bool DoesCurrentActionAllowsMovement()
        {
            return this.PlayerActionExecutionManagerV2.DoesCurrentActionAllowsMovement();
        }

        private bool IsActionOfTypeAllowedToBePlaying(string actionUniqueID)
        {
            return this.PlayerActionExecutionManagerV2.IsActionOfTypeAllowedToBePlaying(actionUniqueID);
        }
        
        #endregion

        #region Data Retrieval

        public PlayerAction GetPlayingPlayerActionReference(string actionUniqueID)
        {
            return this.PlayerActionExecutionManagerV2.GetPlayingPlayerActionReference(actionUniqueID);
        }

        #endregion
    }


    struct PlayerActionState
    {
        private PlayerAction PlayerActionReference;
        private CooldownActionState CooldownActionState;

        public PlayerActionState(PlayerAction playerActionReference, CooldownActionState cooldownActionState)
        {
            PlayerActionReference = playerActionReference;
            CooldownActionState = cooldownActionState;
        }

        #region Logical Conditions

        public bool IsPlayerActionPlaying()
        {
            return this.PlayerActionReference != null;
        }

        public bool IsPlayerActionOnCooldown()
        {
            return this.CooldownActionState.IsOnCooldown();
        }

        public bool CooldownFeatureEnabled()
        {
            return this.CooldownActionState.CooldownFeatureEnabled;
        }

        public bool IsPlayerActionEnded()
        {
            return this.PlayerActionReference != null && this.PlayerActionReference.FinishedCondition();
        }

        #endregion

        public PlayerAction GetPlayerActionReference()
        {
            return this.PlayerActionReference;
        }

        public void ClearPlayerAction()
        {
            this.PlayerActionReference.Dispose();
            this.PlayerActionReference = null;
        }

        public void UpdateCooldown(float d)
        {
            this.CooldownActionState.Tick(d);
        }
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

    #region Action execution

    struct PlayerActionExecutionManagerV2
    {
        private BufferedDictionary<string, PlayerActionState> PlayerActionStates;

        private PlayerActionExecutionLockSystem PlayerActionExecutionLockSystem;
        private PlayerActionEndedCleanupSystem PlayerActionEndedCleanupSystem;
        private PlayerActionCooldownSystem PlayerActionCooldownSystem;

        public void Init()
        {
            this.PlayerActionStates = new BufferedDictionary<string, PlayerActionState>();
            this.PlayerActionExecutionLockSystem = new PlayerActionExecutionLockSystem();
            this.PlayerActionExecutionLockSystem.Init();
            this.PlayerActionEndedCleanupSystem = new PlayerActionEndedCleanupSystem(new Stack<string>());
            this.PlayerActionCooldownSystem = new PlayerActionCooldownSystem();
        }

        public void Execute(PlayerAction playerAction)
        {
            if (this.PlayerActionExecutionLockSystem.ArePlayerActionsLocked())
            {
                this.PlayerActionExecutionLockSystem.AddPlayerActionToLockedQueue(playerAction);
            }
            else
            {
                if (!this.PlayerActionStates.ContainsKey(playerAction.PlayerActionUniqueID))
                {
                    this.PlayerActionStates[playerAction.PlayerActionUniqueID] = new PlayerActionState();
                }

                var playerActionState = this.PlayerActionStates[playerAction.PlayerActionUniqueID];

                if (!playerActionState.IsPlayerActionOnCooldown() && !playerActionState.IsPlayerActionPlaying())
                {
                    playerActionState = new PlayerActionState(playerAction, new CooldownActionState(playerAction.CorePlayerActionDefinition));
                    playerActionState.GetPlayerActionReference().FirstExecution();
                    this.PlayerActionStates[playerAction.PlayerActionUniqueID] = playerActionState;
                }
            }
        }

        public void StopAllActions()
        {
            foreach (var playingPlayerAction in this.PlayerActionStates.Values)
                playingPlayerAction.GetPlayerActionReference()?.Dispose();

            this.PlayerActionStates.Clear();
        }

        public void FixedTick(float d)
        {
            this.InternalUpdate(d, this.InternalFixedTick);
        }

        public void Tick(float d)
        {
            this.InternalUpdate(d, this.InternalTick);
            this.PlayerActionCooldownSystem.UpdateCooldown(d, ref PlayerActionStates);
        }

        public void AfterTicks(float d)
        {
            this.InternalUpdate(d, this.InternalAfterTicks);
        }

        public void TickTimeFrozen(float d)
        {
            this.InternalUpdate(d, this.InternalTickTimeFrozen);
        }

        public void LateTick(float d)
        {
            this.InternalUpdate(d, this.InternalLateTick);
        }

        private void InternalUpdate(float d, Action<float, PlayerActionState> PlayerActionStateUpdate)
        {
            this.PlayerActionExecutionLockSystem.LockPlayerActions();
            foreach (var playerActionState in PlayerActionStates.Values)
            {
                PlayerActionStateUpdate.Invoke(d, playerActionState);
            }

            this.UnlockPlayerActions();
            this.PlayerActionEndedCleanupSystem.CleanupEndedPlayerActions(ref this.PlayerActionStates);
        }

        private void InternalFixedTick(float d, PlayerActionState playerActionState)
        {
            playerActionState.GetPlayerActionReference()?.FixedTick(d);
        }

        private void InternalTick(float d, PlayerActionState playerActionState)
        {
            playerActionState.GetPlayerActionReference()?.Tick(d);
        }

        private void InternalAfterTicks(float d, PlayerActionState playerActionState)
        {
            playerActionState.GetPlayerActionReference()?.AfterTicks(d);
        }

        private void InternalTickTimeFrozen(float d, PlayerActionState playerActionState)
        {
            playerActionState.GetPlayerActionReference()?.TickTimeFrozen(d);
        }

        private void InternalLateTick(float d, PlayerActionState playerActionState)
        {
            playerActionState.GetPlayerActionReference()?.LateTick(d);
        }

        private void UnlockPlayerActions()
        {
            this.PlayerActionExecutionLockSystem.UnlockPlayerActions();
            this.PlayerActionExecutionLockSystem.OnPlayerActionJustUnlocked(in this);
        }

        public bool DoesActionOfTypeIsPlaying(string actionUniqueId)
        {
            return this.PlayerActionStates.ContainsKey(actionUniqueId) && this.PlayerActionStates[actionUniqueId].IsPlayerActionPlaying();
        }

        public bool IsActionOfTypeAllowedToBePlaying(string actionUniqueId)
        {
            return (!this.PlayerActionStates.ContainsKey(actionUniqueId) ||
                    (!this.DoesActionOfTypeIsPlaying(actionUniqueId) && !this.PlayerActionStates[actionUniqueId].IsPlayerActionOnCooldown()));
        }

        public bool DoesCurrentActionAllowsMovement()
        {
            foreach (var playerActionState in PlayerActionStates.Values)
            {
                var playerActionRef = playerActionState.GetPlayerActionReference();
                if (playerActionRef != null && !playerActionRef.MovementAllowed())
                {
                    return false;
                }
            }

            return true;
        }

        public PlayerAction GetPlayingPlayerActionReference(string actionUniqueId)
        {
            this.PlayerActionStates.TryGetValue(actionUniqueId, out PlayerActionState PlayerActionState);
            return PlayerActionState.GetPlayerActionReference();
        }
    }

    /// <summary>
    /// Locks the <see cref="PlayerActionState"/> from <see cref="PlayerActionExecutionManagerV2"/>.
    /// When player actions are locked, newly created player action initialization are deferred after the lock if lift.
    /// This is to prevent modifications of PlayerActionStates of <see cref="PlayerActionExecutionManagerV2"/> when iterating over it.
    /// </summary>
    struct PlayerActionExecutionLockSystem
    {
        private bool CurrentlyPlayerActionsLocked;
        private Stack<PlayerAction> LockedBufferPlayerActionsExecuted;

        public void Init()
        {
            this.CurrentlyPlayerActionsLocked = false;
            this.LockedBufferPlayerActionsExecuted = new Stack<PlayerAction>();
        }

        public void LockPlayerActions()
        {
            this.CurrentlyPlayerActionsLocked = true;
        }

        public void UnlockPlayerActions()
        {
            this.CurrentlyPlayerActionsLocked = false;
        }

        public void AddPlayerActionToLockedQueue(PlayerAction PlayerAction)
        {
            this.LockedBufferPlayerActionsExecuted.Push(PlayerAction);
        }

        #region Logical Conditions

        public bool ArePlayerActionsLocked()
        {
            return this.CurrentlyPlayerActionsLocked;
        }

        #endregion

        public void OnPlayerActionJustUnlocked(in PlayerActionExecutionManagerV2 PlayerActionExecutionManagerV2)
        {
            while (this.LockedBufferPlayerActionsExecuted.Count > 0)
            {
                PlayerActionExecutionManagerV2.Execute(this.LockedBufferPlayerActionsExecuted.Pop());
            }
        }
    }

    /// <summary>
    /// Ends player action when <see cref="PlayerActionState.IsPlayerActionEnded"/> returns true.
    /// </summary>
    struct PlayerActionEndedCleanupSystem
    {
        private Stack<string> StoppedActionTemporaryBuffer;

        public PlayerActionEndedCleanupSystem(Stack<string> StoppedActionTemporaryBuffer)
        {
            this.StoppedActionTemporaryBuffer = StoppedActionTemporaryBuffer;
        }

        public void CleanupEndedPlayerActions(ref BufferedDictionary<string, PlayerActionState> PlayerActionStates)
        {
            foreach (var playerActionStateEntry in PlayerActionStates)
            {
                if (playerActionStateEntry.Value.IsPlayerActionEnded())
                {
                    this.StoppedActionTemporaryBuffer.Push(playerActionStateEntry.Value.GetPlayerActionReference().PlayerActionUniqueID);
                }
            }

            while (this.StoppedActionTemporaryBuffer.Count > 0)
            {
                var stoppedActionID = this.StoppedActionTemporaryBuffer.Pop();
                var stoppedPlayerAction = PlayerActionStates[stoppedActionID];
                stoppedPlayerAction.ClearPlayerAction();
                PlayerActionStates[stoppedActionID] = stoppedPlayerAction;
            }
        }
    }

    /// <summary>
    /// Updates the <see cref="PlayerActionState.CooldownActionState"/>.
    /// </summary>
    struct PlayerActionCooldownSystem
    {
        public void UpdateCooldown(float d, ref BufferedDictionary<string, PlayerActionState> playerActionStates)
        {
            playerActionStates.StartBuffer();

            foreach (var playerActionStateEntry in playerActionStates)
            {
                if (playerActionStateEntry.Value.CooldownFeatureEnabled() && playerActionStateEntry.Value.IsPlayerActionOnCooldown())
                {
                    var playerActionState = playerActionStateEntry.Value;
                    playerActionState.UpdateCooldown(d);
                    playerActionStates.PushToBuffer(new KeyValuePair<string, PlayerActionState>(playerActionStateEntry.Key, playerActionState));
                }
            }

            playerActionStates.UpdateAndConsumeFromBuffer();
        }
    }

    #endregion
}