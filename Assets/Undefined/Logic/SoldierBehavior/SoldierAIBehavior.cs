using System;
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

    public interface ISetAIAgentDestinationActionCallback
    {
        Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction { get; }
        Action<IAgentMovementCalculationStrategy> SetAIAgentDestinationAction_NoReturn { get; }
        Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction { get; }
        Action ClearAIAgentPathAction { get; }
    }

    public interface IFiringProjectileCallback
    {
        Action<Vector3> AskToFireAFiredprojectile_WithWorldDirection_Action { get; }
        Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction { get; }
    }

    public interface IShootingAtPlayerWorkflowCallback
    {
        Action OnShootingAtPlayerStartAction { get; }
        Action OnShootingAtPlayerEndAction { get; }
    }

    public interface IWeaponDataRetrieval
    {
        IWeaponHandlingSystem_DataRetrieval GetIWeaponHandlingSystem_DataRetrievalAction { get; }
    }

    public struct SoldierAIBehaviorExternalCallbacksV2 : ISetAIAgentDestinationActionCallback, IFiringProjectileCallback, IShootingAtPlayerWorkflowCallback, IWeaponDataRetrieval
    {
        public SoldierAIBehaviorExternalCallbacksV2(Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> aiAgentDestinationAction, Action<IAgentMovementCalculationStrategy> aiAgentDestinationActionNoReturn, Action<AIMovementSpeedAttenuationFactor> aiAgentSpeedAttenuationAction, Action clearAiAgentPathAction,
            Action<Vector3> askToFireAFiredprojectileWithWorldDirectionAction, Func<WeaponHandlingFirePointOriginLocalDefinition> weaponFirePointOriginLocalDefinitionAction, Action onShootingAtPlayerStartAction, Action onShootingAtPlayerEndAction, IWeaponHandlingSystem_DataRetrieval iWeaponHandlingSystemDataRetrievalAction)
        {
            SetAIAgentDestinationAction = aiAgentDestinationAction;
            SetAIAgentDestinationAction_NoReturn = aiAgentDestinationActionNoReturn;
            SetAIAgentSpeedAttenuationAction = aiAgentSpeedAttenuationAction;
            ClearAIAgentPathAction = clearAiAgentPathAction;
            AskToFireAFiredprojectile_WithWorldDirection_Action = askToFireAFiredprojectileWithWorldDirectionAction;
            GetWeaponFirePointOriginLocalDefinitionAction = weaponFirePointOriginLocalDefinitionAction;
            OnShootingAtPlayerStartAction = onShootingAtPlayerStartAction;
            OnShootingAtPlayerEndAction = onShootingAtPlayerEndAction;
            GetIWeaponHandlingSystem_DataRetrievalAction = iWeaponHandlingSystemDataRetrievalAction;
        }

        public Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction { get; }
        public Action<IAgentMovementCalculationStrategy> SetAIAgentDestinationAction_NoReturn { get; }
        public Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction { get; }
        public Action ClearAIAgentPathAction { get; }
        public Action<Vector3> AskToFireAFiredprojectile_WithWorldDirection_Action { get; }
        public Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction { get; }
        public Action OnShootingAtPlayerStartAction { get; }
        public Action OnShootingAtPlayerEndAction { get; }
        public IWeaponHandlingSystem_DataRetrieval GetIWeaponHandlingSystem_DataRetrievalAction { get; }


    }

    public class SoldierStateBehavior : StateBehavior<SoldierAIStateEnum, SoldierStateManager>
    {
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        public void Init(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            SoldierAIBehaviorExternalCallbacksV2 SoldierAIBehaviorExternalCallbacksV2
        )
        {
            this.PlayerObjectStateDataSystem = new PlayerObjectStateDataSystem(this.OnPlayerObjectJustOnSight, this.OnPlayerObjectJustOutOfSight);
            this.StateManagersLookup = new Dictionary<SoldierAIStateEnum, SoldierStateManager>()
            {
                {SoldierAIStateEnum.PATROLLING, new PatrollingStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition.AIPatrolGraphBuilder, SoldierAIBehaviorExternalCallbacksV2)},
                {SoldierAIStateEnum.TRACK_AND_KILL_PLAYER, new TrackAndKillPlayerStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition, this.PlayerObjectStateDataSystem, SoldierAIBehaviorExternalCallbacksV2, this.OnAnySubBehaviorAskedToExit)},
                {SoldierAIStateEnum.TRACK_UNKNOWN, new TrackUnknownStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition, SoldierAIBehaviorExternalCallbacksV2, this.OnAnySubBehaviorAskedToExit)}
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