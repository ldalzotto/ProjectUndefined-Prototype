using System.Collections.Generic;
using InteractiveObjects;
using UnityEngine.Profiling;

namespace PlayerActions
{
    public class PlayerActionPlayerSystem
    {
        private PlayerActionExecutionManagerV2 PlayerActionExecutionManagerV2;

        public PlayerActionPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.PlayerActionExecutionManagerV2 = new PlayerActionExecutionManagerV2();
            this.PlayerActionExecutionManagerV2.Init();

            AssociatedInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnAssociatedInteractiveObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : FixedTick");
            this.PlayerActionExecutionManagerV2.FixedTick(d);
            // PlayerActionExecutionManager.FixedTick(d);
            Profiler.EndSample();
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : Tick");
            this.PlayerActionExecutionManagerV2.Tick(d);
            //   PlayerActionExecutionManager.Tick(d);
            Profiler.EndSample();
        }

        public void AfterTicks(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : AfterTicks");
            this.PlayerActionExecutionManagerV2.AfterTicks(d);
            //     PlayerActionExecutionManager.AfterTicks(d);
            Profiler.EndSample();
        }

        public void TickTimeFrozen(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : TickTimeFrozen");
            this.PlayerActionExecutionManagerV2.TickTimeFrozen(d);
            // PlayerActionExecutionManager.TickTimeFrozen(d);
            Profiler.EndSample();
        }

        public void LateTick(float d)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : LateTick");
            this.PlayerActionExecutionManagerV2.LateTick(d);
            //  PlayerActionExecutionManager.LateTick(d);
            Profiler.EndSample();
        }

        public void GizmoTick()
        {
            // PlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            //  PlayerActionExecutionManager.GUITick();
        }

        public void ExecuteAction(PlayerAction rTPPlayerAction)
        {
            Profiler.BeginSample("PlayerActionPlayerSystem : ExecuteAction");
            this.PlayerActionExecutionManagerV2.Execute(rTPPlayerAction);
            //  PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
            Profiler.EndSample();
        }

        #region External Events

        private void OnAssociatedInteractiveObjectDestroyed(CoreInteractiveObject DestroyedInteractiveObject)
        {
            this.PlayerActionExecutionManagerV2.StopAllActions();
            //   this.PlayerActionExecutionManager.StopAllActions();
        }

        #endregion

        #region Logical Conditions

        public bool DoesActionOfTypeIsPlaying(string actionUniqueID)
        {
            return this.PlayerActionExecutionManagerV2.DoesActionOfTypeIsPlaying(actionUniqueID);
            //  return PlayerActionExecutionManager.DoesActionOfTypeIsPlaying(actionType);
        }

        public bool IsActionOfTypeAllowedToBePlaying(string actionUniqueID)
        {
            return this.PlayerActionExecutionManagerV2.IsActionOfTypeAllowedToBePlaying(actionUniqueID);
            //return PlayerActionExecutionManager.IsActionOfTypeAllowedToBePlaying(actionType);
        }

        public bool DoesCurrentActionAllowsMovement()
        {
            return true;
            //return this.PlayerActionExecutionManager.DoesActionsAllowMovement();
        }

        #endregion
    }


    #region Action execution

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
            return this.IsPlayerActionPlaying() && this.CooldownActionState.IsOnCooldown();
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

        public void ClearPlayerActionReference()
        {
            this.PlayerActionReference = null;
        }
    }

    struct PlayerActionExecutionManagerV2
    {
        private Dictionary<string, PlayerActionState> PlayerActionStates;

        private PlayerActionExecutionLockSystem PlayerActionExecutionLockSystem;
        private PlayerActionEndedCleanupSystem PlayerActionEndedCleanupSystem;

        public void Init()
        {
            this.PlayerActionStates = new Dictionary<string, PlayerActionState>();
            this.PlayerActionExecutionLockSystem = new PlayerActionExecutionLockSystem();
            this.PlayerActionExecutionLockSystem.Init();
            this.PlayerActionEndedCleanupSystem = new PlayerActionEndedCleanupSystem(new Stack<string>());
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
            this.PlayerActionExecutionLockSystem.LockPlayerActions();

            foreach (var playerActionState in PlayerActionStates.Values)
            {
                playerActionState.GetPlayerActionReference()?.FixedTick(d);
            }

            this.UnlockPlayerActions();

            this.PlayerActionEndedCleanupSystem.CleanupEndedPlayerActions(ref this.PlayerActionStates);
        }

        public void Tick(float d)
        {
            this.PlayerActionExecutionLockSystem.LockPlayerActions();

            foreach (var playerActionState in PlayerActionStates.Values)
            {
                playerActionState.GetPlayerActionReference()?.Tick(d);
            }

            this.UnlockPlayerActions();

            this.PlayerActionEndedCleanupSystem.CleanupEndedPlayerActions(ref this.PlayerActionStates);
        }

        public void AfterTicks(float d)
        {
            this.PlayerActionExecutionLockSystem.LockPlayerActions();

            foreach (var playerActionState in PlayerActionStates.Values)
            {
                playerActionState.GetPlayerActionReference()?.AfterTicks(d);
            }

            this.UnlockPlayerActions();

            this.PlayerActionEndedCleanupSystem.CleanupEndedPlayerActions(ref this.PlayerActionStates);
        }

        public void TickTimeFrozen(float d)
        {
            this.PlayerActionExecutionLockSystem.LockPlayerActions();

            foreach (var playerActionState in PlayerActionStates.Values)
            {
                playerActionState.GetPlayerActionReference()?.TickTimeFrozen(d);
            }

            this.UnlockPlayerActions();

            this.PlayerActionEndedCleanupSystem.CleanupEndedPlayerActions(ref this.PlayerActionStates);
        }

        public void LateTick(float d)
        {
            this.PlayerActionExecutionLockSystem.LockPlayerActions();

            foreach (var playerActionState in PlayerActionStates.Values)
            {
                playerActionState.GetPlayerActionReference()?.LateTick(d);
            }

            this.UnlockPlayerActions();

            this.PlayerActionEndedCleanupSystem.CleanupEndedPlayerActions(ref this.PlayerActionStates);
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
            return !this.DoesActionOfTypeIsPlaying(actionUniqueId)
                   && (!this.PlayerActionStates.ContainsKey(actionUniqueId) || (this.PlayerActionStates.ContainsKey(actionUniqueId) && !this.PlayerActionStates[actionUniqueId].IsPlayerActionOnCooldown()));
        }
    }

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

    struct PlayerActionEndedCleanupSystem
    {
        private Stack<string> StoppedActionTemporaryBuffer;

        public PlayerActionEndedCleanupSystem(Stack<string> StoppedActionTemporaryBuffer)
        {
            this.StoppedActionTemporaryBuffer = StoppedActionTemporaryBuffer;
        }

        public void CleanupEndedPlayerActions(ref Dictionary<string, PlayerActionState> PlayerActionStates)
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
                stoppedPlayerAction.GetPlayerActionReference().Dispose();
                stoppedPlayerAction.ClearPlayerActionReference();
                PlayerActionStates[stoppedActionID] = stoppedPlayerAction;
            }
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

    #endregion
}