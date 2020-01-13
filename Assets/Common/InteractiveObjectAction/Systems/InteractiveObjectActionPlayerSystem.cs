using System;
using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using UnityEngine;
using UnityEngine.Profiling;

namespace InteractiveObjectAction
{
    public class InteractiveObjectActionPlayerSystem
    {
        private InteractiveObjectActionExecutionManager interactiveObjectActionExecutionManager;
        private CoreInteractiveObject AssociatedInteractiveObject;

        public InteractiveObjectActionPlayerSystem(CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.interactiveObjectActionExecutionManager = new InteractiveObjectActionExecutionManager();
            this.interactiveObjectActionExecutionManager.Init();

            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            AssociatedInteractiveObject.RegisterInteractiveObjectDestroyedEventListener(this.OnAssociatedInteractiveObjectDestroyed);
        }

        public void FixedTick(float d)
        {
            this.interactiveObjectActionExecutionManager.FixedTick(d);
        }

        public void Tick(float d)
        {
            this.interactiveObjectActionExecutionManager.Tick(d);
        }

        public void AfterTicks(float d)
        {
            this.interactiveObjectActionExecutionManager.AfterTicks(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.interactiveObjectActionExecutionManager.TickTimeFrozen(d);
        }

        public void LateTick(float d)
        {
            this.interactiveObjectActionExecutionManager.LateTick(d);
        }

        public void GizmoTick()
        {
            // PlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            //  PlayerActionExecutionManager.GUITick();
        }

        public void ExecuteActionV2(InteractiveObjectActionInherentData interactiveObjectActionInherentData)
        {
            if (this.IsActionOfTypeAllowedToBePlaying(interactiveObjectActionInherentData.InteractiveObjectActionUniqueID))
            {
                var playerActionInput = interactiveObjectActionInherentData.BuildInputFromInteractiveObject(this.AssociatedInteractiveObject);
                if (playerActionInput != null)
                {
                    this.interactiveObjectActionExecutionManager.Execute(interactiveObjectActionInherentData.BuildInteractiveObjectAction(playerActionInput));
                }
            }
        }

        #region External Events

        private void OnAssociatedInteractiveObjectDestroyed(CoreInteractiveObject DestroyedInteractiveObject)
        {
            this.interactiveObjectActionExecutionManager.StopAllActions();
        }

        #endregion

        #region Logical Conditions

        public bool DoesCurrentActionAllowsMovement()
        {
            return this.interactiveObjectActionExecutionManager.DoesCurrentActionAllowsMovement();
        }

        #endregion

        private bool IsActionOfTypeAllowedToBePlaying(string actionUniqueID)
        {
            return this.interactiveObjectActionExecutionManager.IsActionOfTypeAllowedToBePlaying(actionUniqueID);
        }

        #region Data Retrieval

        public AInteractiveObjectAction GetPlayingPlayerActionReference(string actionUniqueID)
        {
            return this.interactiveObjectActionExecutionManager.GetPlayingInteractiveObjectActionReference(actionUniqueID);
        }

        public float GetCooldownPercentageOfPlayerAction(string actionUniqueID)
        {
            return this.interactiveObjectActionExecutionManager.GetCooldownPercentageOfInteractiveObjectAction(actionUniqueID);
        }

        #endregion
    }

    /// <summary>
    /// The state associated to every unique <see cref="AInteractiveObjectAction.InteractiveObjectActionUniqueID"/>.
    /// </summary>
    struct InteractiveObjectActionState
    {
        /// <summary>
        /// The reference of the currently executed instance of <see cref="AInteractiveObjectAction"/>.
        /// /!\ This value is set to null when either the <see cref="AInteractiveObjectAction"/> has ended (called from <see cref="InteractiveObjectActionEndedCleanupSystem"/>) or
        ///     the <see cref="InteractiveObjectActionPlayerSystem"/> is manually disposed.
        /// A value of null thus indicates the the action is currenlty not running. <see cref="IsInteractiveObjectActionCurrentlyRunning"/>
        /// </summary>
        private AInteractiveObjectAction _aInteractiveObjectActionReference;

        private CooldownActionState CooldownActionState;

        public InteractiveObjectActionState(AInteractiveObjectAction aInteractiveObjectActionReference, CooldownActionState cooldownActionState)
        {
            _aInteractiveObjectActionReference = aInteractiveObjectActionReference;
            CooldownActionState = cooldownActionState;
        }

        #region Logical Conditions

        /// <summary>
        /// <see cref="_aInteractiveObjectActionReference"/> for more info.
        /// </summary>
        public bool IsInteractiveObjectActionCurrentlyRunning()
        {
            return this._aInteractiveObjectActionReference != null;
        }

        public bool IsInteractiveObjectActionEnded()
        {
            return this.IsInteractiveObjectActionCurrentlyRunning() && this._aInteractiveObjectActionReference.FinishedCondition();
        }

        public bool IsInteractiveObjectActionOnCooldown()
        {
            return this.CooldownActionState.IsOnCooldown();
        }

        public bool CooldownFeatureEnabled()
        {
            return this.CooldownActionState.CooldownFeatureEnabled;
        }

        #endregion

        public AInteractiveObjectAction GetInteractiveObjectActionReference()
        {
            return this._aInteractiveObjectActionReference;
        }

        /// <summary>
        /// Called from <see cref="InteractiveObjectActionEndedCleanupSystem"/> when the <see cref="_aInteractiveObjectActionReference"/> is considered done.
        /// </summary>
        public void ClearInteractiveObjectAction()
        {
            this._aInteractiveObjectActionReference.Dispose();
            this._aInteractiveObjectActionReference = null;
        }

        public void UpdateCooldown(float d)
        {
            this.CooldownActionState.Tick(d);
        }

        public float Get01CooldownCompletion()
        {
            return this.CooldownActionState.Get01CooldownCompletion();
        }
    }

    struct CooldownActionState
    {
        public bool CooldownFeatureEnabled;

        /// <summary>
        /// The elasped time starting from which the <see cref="CooldownActionState"/> is no more considered on cooldown <see cref="IsOnCooldown"/>
        /// </summary>
        private float TargetCooldownTime;

        private float CurrentTimeElapsed;

        public CooldownActionState(CoreInteractiveObjectActionDefinition coreInteractiveObjectActionDefinition)
        {
            this.CooldownFeatureEnabled = coreInteractiveObjectActionDefinition.CooldownEnabled;
            this.TargetCooldownTime = 0f;

            if (this.CooldownFeatureEnabled)
            {
                TargetCooldownTime = coreInteractiveObjectActionDefinition.interactiveObjectActionCooldownDefinition.CoolDownTime;
            }

            CurrentTimeElapsed = 0f;
        }

        /// <summary>
        /// /!\ This tick is called from <see cref="InteractiveObjectActionCooldownSystem"/> and only called when the <see cref="CooldownActionState"/> is considered on cooldown.
        /// See <see cref="InteractiveObjectActionCooldownSystem.UpdateCooldown"/> for implementation.
        /// </summary>
        public void Tick(float d)
        {
            this.CurrentTimeElapsed += d;
        }

        public bool IsOnCooldown()
        {
            return this.CooldownFeatureEnabled && (this.TargetCooldownTime > 0f && this.CurrentTimeElapsed <= this.TargetCooldownTime);
        }

        /// <summary>
        /// 0 means that the cooldown has just started.
        /// 1 means that the cooldown has ended
        /// </summary>
        public float Get01CooldownCompletion()
        {
            if (this.TargetCooldownTime == 0f)
            {
                return 1f;
            }

            return 1 - Mathf.Clamp01(((this.TargetCooldownTime - this.CurrentTimeElapsed) / this.TargetCooldownTime));
        }
    }

    #region Action execution

    /// <summary>
    /// Is in charge of updating all <see cref="AInteractiveObjectAction"/> associated to <see cref="InteractiveObjectActionState"/>.
    /// </summary>
    struct InteractiveObjectActionExecutionManager
    {
        private BufferedDictionary<string, InteractiveObjectActionState> InteractiveObjectActionStates;

        private InteractiveObjectActionExecutionLockSystem _interactiveObjectActionExecutionLockSystem;
        private InteractiveObjectActionEndedCleanupSystem _interactiveObjectActionEndedCleanupSystem;
        private InteractiveObjectActionCooldownSystem _interactiveObjectActionCooldownSystem;

        public void Init()
        {
            this.InteractiveObjectActionStates = new BufferedDictionary<string, InteractiveObjectActionState>();
            this._interactiveObjectActionExecutionLockSystem = new InteractiveObjectActionExecutionLockSystem();
            this._interactiveObjectActionExecutionLockSystem.Init();
            this._interactiveObjectActionEndedCleanupSystem = new InteractiveObjectActionEndedCleanupSystem(new Stack<string>());
            this._interactiveObjectActionCooldownSystem = new InteractiveObjectActionCooldownSystem();
        }

        public void Execute(AInteractiveObjectAction aInteractiveObjectAction)
        {
            if (this._interactiveObjectActionExecutionLockSystem.AreInteractiveObjectActionsLocked())
            {
                this._interactiveObjectActionExecutionLockSystem.AddInteractiveObjectActionToLockedQueue(aInteractiveObjectAction);
            }
            else
            {
                if (!this.InteractiveObjectActionStates.ContainsKey(aInteractiveObjectAction.InteractiveObjectActionUniqueID))
                {
                    this.InteractiveObjectActionStates[aInteractiveObjectAction.InteractiveObjectActionUniqueID] = new InteractiveObjectActionState();
                }

                var InteractiveObjectActionState = this.InteractiveObjectActionStates[aInteractiveObjectAction.InteractiveObjectActionUniqueID];

                if (!InteractiveObjectActionState.IsInteractiveObjectActionOnCooldown() && !InteractiveObjectActionState.IsInteractiveObjectActionCurrentlyRunning())
                {
                    InteractiveObjectActionState = new InteractiveObjectActionState(aInteractiveObjectAction, new CooldownActionState(aInteractiveObjectAction.CoreInteractiveObjectActionDefinition));
                    InteractiveObjectActionState.GetInteractiveObjectActionReference().FirstExecution();
                    this.InteractiveObjectActionStates[aInteractiveObjectAction.InteractiveObjectActionUniqueID] = InteractiveObjectActionState;
                }
            }
        }

        public void StopAllActions()
        {
            foreach (var playingInteractiveObjectAction in this.InteractiveObjectActionStates.Values)
                playingInteractiveObjectAction.GetInteractiveObjectActionReference()?.Dispose();

            this.InteractiveObjectActionStates.Clear();
        }

        /// <summary>
        /// The core update loop.
        /// Cooldown are update only one per frame in <see cref="Tick"/>.
        /// </summary>
        private void InternalUpdate(float d, Action<float, InteractiveObjectActionState> InteractiveObjectActionStateUpdate)
        {
            this.LockInteractiveObjectActions();

            foreach (var InteractiveObjectActionState in InteractiveObjectActionStates.Values)
            {
                InteractiveObjectActionStateUpdate.Invoke(d, InteractiveObjectActionState);
            }

            this.UnlockInteractiveObjectActions();

            this._interactiveObjectActionEndedCleanupSystem.CleanupEndedInteractiveObjectActions(ref this.InteractiveObjectActionStates);
        }

        public void FixedTick(float d)
        {
            this.InternalUpdate(d, this.InternalFixedTick);
        }

        public void Tick(float d)
        {
            this.InternalUpdate(d, this.InternalTick);
            this._interactiveObjectActionCooldownSystem.UpdateCooldown(d, ref InteractiveObjectActionStates);
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

        private void InternalFixedTick(float d, InteractiveObjectActionState interactiveObjectActionState)
        {
            interactiveObjectActionState.GetInteractiveObjectActionReference()?.FixedTick(d);
        }

        private void InternalTick(float d, InteractiveObjectActionState interactiveObjectActionState)
        {
            interactiveObjectActionState.GetInteractiveObjectActionReference()?.Tick(d);
        }

        private void InternalAfterTicks(float d, InteractiveObjectActionState interactiveObjectActionState)
        {
            interactiveObjectActionState.GetInteractiveObjectActionReference()?.AfterTicks(d);
        }

        private void InternalTickTimeFrozen(float d, InteractiveObjectActionState interactiveObjectActionState)
        {
            interactiveObjectActionState.GetInteractiveObjectActionReference()?.TickTimeFrozen(d);
        }

        private void InternalLateTick(float d, InteractiveObjectActionState interactiveObjectActionState)
        {
            interactiveObjectActionState.GetInteractiveObjectActionReference()?.LateTick(d);
        }

        private void LockInteractiveObjectActions()
        {
            this._interactiveObjectActionExecutionLockSystem.LockInteractiveObjectActions();
        }

        private void UnlockInteractiveObjectActions()
        {
            this._interactiveObjectActionExecutionLockSystem.UnlockInteractiveObjectActions();
            this._interactiveObjectActionExecutionLockSystem.OnInteractiveObjectActionJustUnlocked(in this);
        }

        /// <summary>
        /// The <see cref="AInteractiveObjectAction"/> is considered to be playing if :
        /// * The <see cref="InteractiveObjectActionStates"/> contains the uniqueID. This means that the <see cref="AInteractiveObjectAction"/> has been played at least once.
        /// * An instance of the <see cref="AInteractiveObjectAction"/> is currently running.
        /// </summary>
        public bool DoesActionOfTypeIsPlaying(string actionUniqueId)
        {
            return this.InteractiveObjectActionStates.ContainsKey(actionUniqueId) && this.InteractiveObjectActionStates[actionUniqueId].IsInteractiveObjectActionCurrentlyRunning();
        }

        /// <summary>
        /// The <see cref="AInteractiveObjectAction"/> is considered to be allowed to be played if :
        /// * The <see cref="InteractiveObjectActionStates"/> doesn't contains the uniqueID. This means that the <see cref="AInteractiveObjectAction"/> has not already been played.
        /// * An instance of the <see cref="AInteractiveObjectAction"/> is not currently running. Only one instance of every <see cref="AInteractiveObjectAction"/> can run simultaneously
        /// * The <see cref="AInteractiveObjectAction"/> not currently running is not on cooldown.
        /// </summary>
        public bool IsActionOfTypeAllowedToBePlaying(string actionUniqueId)
        {
            return (!this.InteractiveObjectActionStates.ContainsKey(actionUniqueId) ||
                    (!this.DoesActionOfTypeIsPlaying(actionUniqueId) && !this.InteractiveObjectActionStates[actionUniqueId].IsInteractiveObjectActionOnCooldown()));
        }

        public bool DoesCurrentActionAllowsMovement()
        {
            foreach (var InteractiveObjectActionState in InteractiveObjectActionStates.Values)
            {
                var InteractiveObjectActionRef = InteractiveObjectActionState.GetInteractiveObjectActionReference();
                if (InteractiveObjectActionRef != null && !InteractiveObjectActionRef.MovementAllowed())
                {
                    return false;
                }
            }

            return true;
        }

        public AInteractiveObjectAction GetPlayingInteractiveObjectActionReference(string actionUniqueId)
        {
            this.InteractiveObjectActionStates.TryGetValue(actionUniqueId, out InteractiveObjectActionState InteractiveObjectActionState);
            return InteractiveObjectActionState.GetInteractiveObjectActionReference();
        }

        public float GetCooldownPercentageOfInteractiveObjectAction(string actionUniqueId)
        {
            this.InteractiveObjectActionStates.TryGetValue(actionUniqueId, out InteractiveObjectActionState InteractiveObjectActionState);
            return InteractiveObjectActionState.Get01CooldownCompletion();
        }
    }

    /// <summary>
    /// Locks the <see cref="InteractiveObjectActionState"/> from <see cref="InteractiveObjectActionExecutionManager"/>.
    /// When actions are locked, newly created action initialization are deferred after the lock is lift.
    /// This is to prevent modifications of InteractiveObjectActionStates of <see cref="InteractiveObjectActionExecutionManager"/> when iterating over it.
    /// </summary>
    struct InteractiveObjectActionExecutionLockSystem
    {
        private bool CurrentlyInteractiveObjectActionsLocked;
        private Stack<AInteractiveObjectAction> LockedBufferInteractiveObjectActionsExecuted;

        public void Init()
        {
            this.CurrentlyInteractiveObjectActionsLocked = false;
            this.LockedBufferInteractiveObjectActionsExecuted = new Stack<AInteractiveObjectAction>();
        }

        public void LockInteractiveObjectActions()
        {
            this.CurrentlyInteractiveObjectActionsLocked = true;
        }

        public void UnlockInteractiveObjectActions()
        {
            this.CurrentlyInteractiveObjectActionsLocked = false;
        }

        public void AddInteractiveObjectActionToLockedQueue(AInteractiveObjectAction aInteractiveObjectAction)
        {
            this.LockedBufferInteractiveObjectActionsExecuted.Push(aInteractiveObjectAction);
        }

        #region Logical Conditions

        public bool AreInteractiveObjectActionsLocked()
        {
            return this.CurrentlyInteractiveObjectActionsLocked;
        }

        #endregion

        public void OnInteractiveObjectActionJustUnlocked(in InteractiveObjectActionExecutionManager interactiveObjectActionExecutionManager)
        {
            while (this.LockedBufferInteractiveObjectActionsExecuted.Count > 0)
            {
                interactiveObjectActionExecutionManager.Execute(this.LockedBufferInteractiveObjectActionsExecuted.Pop());
            }
        }
    }

    /// <summary>
    /// Clear the <see cref="InteractiveObjectActionState"/> (by calling <see cref="InteractiveObjectActionState.ClearInteractiveObjectAction"/>)
    /// when it's associated <see cref="AInteractiveObjectAction"/> is considered done.
    /// </summary>
    struct InteractiveObjectActionEndedCleanupSystem
    {
        private Stack<string> StoppedActionTemporaryBuffer;

        public InteractiveObjectActionEndedCleanupSystem(Stack<string> StoppedActionTemporaryBuffer)
        {
            this.StoppedActionTemporaryBuffer = StoppedActionTemporaryBuffer;
        }

        public void CleanupEndedInteractiveObjectActions(ref BufferedDictionary<string, InteractiveObjectActionState> InteractiveObjectActionStates)
        {
            foreach (var InteractiveObjectActionStateEntry in InteractiveObjectActionStates)
            {
                if (InteractiveObjectActionStateEntry.Value.IsInteractiveObjectActionEnded())
                {
                    this.StoppedActionTemporaryBuffer.Push(InteractiveObjectActionStateEntry.Value.GetInteractiveObjectActionReference().InteractiveObjectActionUniqueID);
                }
            }

            while (this.StoppedActionTemporaryBuffer.Count > 0)
            {
                var stoppedActionID = this.StoppedActionTemporaryBuffer.Pop();
                var stoppedInteractiveObjectAction = InteractiveObjectActionStates[stoppedActionID];
                stoppedInteractiveObjectAction.ClearInteractiveObjectAction();
                InteractiveObjectActionStates[stoppedActionID] = stoppedInteractiveObjectAction;
            }
        }
    }

    /// <summary>
    /// Updates the <see cref="InteractiveObjectActionState.CooldownActionState"/>.
    /// </summary>
    struct InteractiveObjectActionCooldownSystem
    {
        /// <summary>
        /// /!\ Warning, this method must be called once per frame (or with <paramref name="d"/> as 0).
        /// Otherwise that would cause the cooldown timer to be updated multiple time per frames
        /// </summary>
        public void UpdateCooldown(float d, ref BufferedDictionary<string, InteractiveObjectActionState> InteractiveObjectActionStates)
        {
            InteractiveObjectActionStates.StartBuffer();

            foreach (var InteractiveObjectActionStateEntry in InteractiveObjectActionStates)
            {
                /// Only InteractiveObjectActionState that have cooldown enabled and is on cooldown are updated
                if (InteractiveObjectActionStateEntry.Value.CooldownFeatureEnabled() && InteractiveObjectActionStateEntry.Value.IsInteractiveObjectActionOnCooldown())
                {
                    var InteractiveObjectActionState = InteractiveObjectActionStateEntry.Value;
                    InteractiveObjectActionState.UpdateCooldown(d);
                    InteractiveObjectActionStates.PushToBuffer(new KeyValuePair<string, InteractiveObjectActionState>(InteractiveObjectActionStateEntry.Key, InteractiveObjectActionState));
                }
            }

            InteractiveObjectActionStates.UpdateAndConsumeFromBuffer();
        }
    }

    #endregion
}