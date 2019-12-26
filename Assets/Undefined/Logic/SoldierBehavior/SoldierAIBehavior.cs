﻿using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using RangeObjects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;
using Weapon;

namespace SoliderAIBehavior
{
    public enum SoldierAIStateEnum
    {
        PATROLLING,
        TRACK_AND_KILL_PLAYER,
        TRACK_UNKNOWN
    }

    public struct SoldierAIBehaviorExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction;

        /// <summary>
        /// Clear the path of <see cref="AIObjects.AIMoveToDestinationSystem"/>
        /// </summary>
        public Action ClearAIAgentPathAction;

        public Action<CoreInteractiveObject> AskToFireAFiredProjectile_WithTargetPosition_Action;
        public Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction;

        public Action OnShootingAtPlayerStartAction;
        public Action OnShootingAtPlayerEndAction;

        public SoldierAIBehaviorExternalCallbacks(Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction,
            Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction, Action clearAiAgentPathAction, Action<CoreInteractiveObject> askToFireAFiredProjectileWithTargetPositionAction, Func<WeaponHandlingFirePointOriginLocalDefinition> weaponFirePointOriginLocalDefinitionAction, Action onShootingAtPlayerStartAction,
            Action onShootingAtPlayerEndAction)
        {
            this.SetAIAgentDestinationAction = SetAIAgentDestinationAction;
            this.SetAIAgentSpeedAttenuationAction = SetAIAgentSpeedAttenuationAction;
            ClearAIAgentPathAction = clearAiAgentPathAction;
            AskToFireAFiredProjectile_WithTargetPosition_Action = askToFireAFiredProjectileWithTargetPositionAction;
            GetWeaponFirePointOriginLocalDefinitionAction = weaponFirePointOriginLocalDefinitionAction;
            OnShootingAtPlayerStartAction = onShootingAtPlayerStartAction;
            OnShootingAtPlayerEndAction = onShootingAtPlayerEndAction;
        }
    }

    public class SoldierStateBehavior : StateBehavior<SoldierAIStateEnum, SoldierStateManager>
    {
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        public void Init(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            SoldierAIBehaviorExternalCallbacks SoldierAIBehaviorExternalCallbacks
        )
        {
            this.PlayerObjectStateDataSystem = new PlayerObjectStateDataSystem(this.OnPlayerObjectJustOnSight, this.OnPlayerObjectJustOutOfSight);
            this.StateManagersLookup = new Dictionary<SoldierAIStateEnum, SoldierStateManager>()
            {
                {SoldierAIStateEnum.PATROLLING, new PatrollingStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition.AIPatrolSystemDefinition, SoldierAIBehaviorExternalCallbacks.SetAIAgentDestinationAction, SoldierAIBehaviorExternalCallbacks.SetAIAgentSpeedAttenuationAction)},
                {SoldierAIStateEnum.TRACK_AND_KILL_PLAYER, new TrackAndKillPlayerStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition, this.PlayerObjectStateDataSystem, SoldierAIExternalCallbacksStructureConverter.Map2TrackAndKillPlayerStateManagerExternalCallbacks(SoldierAIBehaviorExternalCallbacks, this))},
                {SoldierAIStateEnum.TRACK_UNKNOWN, new TrackUnknownStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition, SoldierAIExternalCallbacksStructureConverter.Map2TrackUnknownStateManagerExternalCallbacks(SoldierAIBehaviorExternalCallbacks, this))}
            };

            base.Init(SoldierAIStateEnum.PATROLLING);
        }

        public override void Tick(float d)
        {
            Profiler.BeginSample("SoldierAIBehavior");
            this.PlayerObjectStateDataSystem.Tick(d);
            base.Tick(d);
            Profiler.EndSample();
        }

        public override void SetState(SoldierAIStateEnum NewState)
        {
            base.SetState(NewState);
        }

        public override void OnDestroy()
        {
            foreach (var stateManager in this.StateManagersLookup.Values)
            {
                stateManager.OnDestroy();
            }
        }

        #region External Sight Events

        public void OnInteractiveObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            this.PlayerObjectStateDataSystem.OnInteractiveObjectJustOnSight(InSightInteractiveObject);
        }

        public void OnInteractiveObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            this.PlayerObjectStateDataSystem.OnInteractiveObjectJustOutOfSight(NotInSightInteractiveObject);
        }

        #endregion

        #region Internal Sight Events

        private void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            this.SetState(SoldierAIStateEnum.TRACK_AND_KILL_PLAYER);
            this.GetCurrentStateManager().OnPlayerObjectJustOnSight(InSightInteractiveObject);
        }

        private void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            this.GetCurrentStateManager().OnPlayerObjectJustOutOfSight(NotInSightInteractiveObject);
        }

        #endregion

        #region Internal Sub Behaviors Events

        /// <summary>
        /// When a <see cref="SoldierStateManager"/> call this event, this means that nothings has happened and he wants
        /// to return to <see cref="SoldierStateBehavior"/> default state.
        /// </summary>
        private void OnAnySubBehaviorAskedToExit()
        {
            this.SetState(SoldierAIStateEnum.PATROLLING);
        }

        #endregion

        #region External Agent Events

        public void OnDestinationReached()
        {
            this.GetCurrentStateManager().OnDestinationReached();
        }

        #endregion

        #region External Health Events

        /// <summary>
        /// It is crutial to set the set the SoldierAIStateEnum.TRACK_UNKNOWN state before propagating
        /// DamageDealt event to execute DamageDealt logic for the new state.
        /// </summary>
        public void DamageDealt(CoreInteractiveObject DamageDealerInteractiveObject)
        {
            /// "If the player is not in sight" 
            if (!this.PlayerObjectStateDataSystem.IsPlayerInSight.GetValue())
            {
                Debug.Log(MyLog.Format("Switch to TRACK_UNKNOWN"));
                this.SetState(SoldierAIStateEnum.TRACK_UNKNOWN);
            }

            this.GetCurrentStateManager().DamageDealt(DamageDealerInteractiveObject);
        }

        #endregion

        public static class SoldierAIExternalCallbacksStructureConverter
        {
            public static TrackAndKillPlayerStateManagerExternalCallbacks Map2TrackAndKillPlayerStateManagerExternalCallbacks(SoldierAIBehaviorExternalCallbacks SoldierAIBehaviorExternalCallbacks, SoldierStateBehavior soldierStateBehavior)
            {
                return new TrackAndKillPlayerStateManagerExternalCallbacks(
                    SoldierAIBehaviorExternalCallbacks.SetAIAgentDestinationAction,
                    SoldierAIBehaviorExternalCallbacks.SetAIAgentSpeedAttenuationAction,
                    SoldierAIBehaviorExternalCallbacks.ClearAIAgentPathAction,
                    SoldierAIBehaviorExternalCallbacks.AskToFireAFiredProjectile_WithTargetPosition_Action,
                    SoldierAIBehaviorExternalCallbacks.GetWeaponFirePointOriginLocalDefinitionAction,
                    soldierStateBehavior.OnAnySubBehaviorAskedToExit,
                    SoldierAIBehaviorExternalCallbacks.OnShootingAtPlayerStartAction,
                    SoldierAIBehaviorExternalCallbacks.OnShootingAtPlayerEndAction
                );
            }

            public static TrackUnknownStateManagerExternalCallbacks Map2TrackUnknownStateManagerExternalCallbacks(SoldierAIBehaviorExternalCallbacks SoldierAIBehaviorExternalCallbacks, SoldierStateBehavior soldierStateBehavior)
            {
                return new TrackUnknownStateManagerExternalCallbacks(
                    SoldierAIBehaviorExternalCallbacks.SetAIAgentDestinationAction,
                    SoldierAIBehaviorExternalCallbacks.SetAIAgentSpeedAttenuationAction,
                    soldierStateBehavior.OnAnySubBehaviorAskedToExit
                );
            }
        }
    }

    /// <summary>
    /// Base class of all Solider AI <see cref="StateManager"/>. It defines common events functions that can be implemented
    /// by any <see cref="StateManager"/>.
    /// This is to avoid having branching condition when an event is emitted to <see cref="SoldierStateBehavior"/>
    /// </summary>
    public abstract class SoldierStateManager : StateManager
    {
        /// <summary>
        /// This event is called just when Player has been on sight.
        /// The call is done during the <see cref="RangeIntersectionV2System"/> calculation step. This cause the <see cref="PlayerObjectStateDataSystem.LastPlayerSeenPosition"/>
        /// to not have been updated when the event is called.
        /// It is often a better choice to execute the logic in the <see cref="StateManager.Tick"/> method by checking
        /// <see cref="PlayerObjectStateDataSystem.IsPlayerInSight"/> state. 
        /// </summary>
        /// <param name="InSightInteractiveObject"></param>
        public virtual void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
        }

        public virtual void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
        }

        /// <summary>
        /// Usually called from <see cref="AIMoveToDestinationSystem"/> when the <see cref="NavMeshAgent"/> has readched it's
        /// current destination.
        /// </summary>
        public virtual void OnDestinationReached()
        {
        }

        /// <summary>
        /// This event is called when a damge from the <see cref="CoreInteractiveObject.DealDamage"/> event.
        /// </summary>
        public virtual void DamageDealt(CoreInteractiveObject damageDealerInteractiveObject)
        {
        }

        public virtual void OnDestroy()
        {
        }
    }


    public static class SoldierAIBehaviorUtil
    {
        public static bool PlayerInSightButNoObstaclesBetween(PlayerObjectStateDataSystem PlayerObjectStateDataSystem, WeaponFiringAreaSystem WeaponFiringAreaSystem)
        {
            return PlayerObjectStateDataSystem.IsPlayerInSight.GetValue() && !WeaponFiringAreaSystem.AreObstaclesInside();
        }
    }
}