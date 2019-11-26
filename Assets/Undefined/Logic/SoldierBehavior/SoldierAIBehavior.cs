using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using RangeObjects;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace SoliderAIBehavior
{
    public enum SoldierAIStateEnum
    {
        PATROLLING,
        TRACK_AND_KILL_PLAYER,
        TRACK_UNKNOWN
    }

    public class SoldierAIBehavior : AIBehavior<SoldierAIStateEnum, SoldierStateManager>
    {
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        public SoldierAIBehavior(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            Func<IAgentMovementCalculationStrategy, AIMovementSpeedAttenuationFactor, NavMeshPathStatus> destinationAction, Action ClearpathAction, Action<Vector3> AskToFireAFiredProjectileAction_WithTargetPosition,
            Func<WeaponHandlingFirePointOriginLocalDefinition> GetWeaponFirePointOriginLocalDefinitionAction
        ) : base(SoldierAIStateEnum.PATROLLING)
        {
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.PlayerObjectStateDataSystem = new PlayerObjectStateDataSystem(this.OnPlayerObjectJustOnSight, this.OnPlayerObjectJustOutOfSight);
            this.StateManagersLookup = new Dictionary<SoldierAIStateEnum, SoldierStateManager>()
            {
                {SoldierAIStateEnum.PATROLLING, new PatrollingStateManager( AssociatedInteractiveObject, SoldierAIBehaviorDefinition.AIPatrolSystemDefinition)},
                {
                    SoldierAIStateEnum.TRACK_AND_KILL_PLAYER, new TrackAndKillPlayerStateManager(
                        AssociatedInteractiveObject, SoldierAIBehaviorDefinition, this.PlayerObjectStateDataSystem, destinationAction, ClearpathAction,
                        AskToFireAFiredProjectileAction_WithTargetPosition, GetWeaponFirePointOriginLocalDefinitionAction, this.OnAnySubBehaviorAskedToExit)
                },
                {SoldierAIStateEnum.TRACK_UNKNOWN, new TrackUnknownStateManager(AssociatedInteractiveObject, SoldierAIBehaviorDefinition, destinationAction, this.OnAnySubBehaviorAskedToExit)}
            };
        }

        public override void Tick(float d)
        {
            this.PlayerObjectStateDataSystem.Tick(d);
            base.Tick(d);
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
        /// to return to <see cref="SoldierAIBehavior"/> default state.
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
            if (!this.PlayerObjectStateDataSystem.IsPlayerInSight)
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
    /// This is to avoid having branching condition when an event is emitted to <see cref="SoldierAIBehavior"/>
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
        public static bool InteractiveObjectBeyondObstacle(CoreInteractiveObject InteractiveObject, CoreInteractiveObject SoliderEnemy)
        {
            var NotInSightInteractiveObjectWorldPos = InteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
            var AssociatedInteractiveobjectWorldPos = SoliderEnemy.InteractiveGameObject.GetTransform().WorldPosition;
            float DistanceFromAssociatedInteractiveObject = Vector3.Distance(NotInSightInteractiveObjectWorldPos, AssociatedInteractiveobjectWorldPos);
            return Physics.Raycast(NotInSightInteractiveObjectWorldPos, (AssociatedInteractiveobjectWorldPos - NotInSightInteractiveObjectWorldPos).normalized, DistanceFromAssociatedInteractiveObject, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES));
        }

        /// <summary>
        /// This method must be the condition guard before entering <see cref="SoldierAIStateEnum.SHOOTING_AT_PLAYER"/> state.
        /// </summary>
        public static bool IsAllowToMoveToShootingAtPlayerState(PlayerObjectStateDataSystem PlayerObjectStateDataSystem, WeaponFiringAreaSystem WeaponFiringAreaSystem)
        {
            return PlayerObjectStateDataSystem.IsPlayerInSight && !WeaponFiringAreaSystem.AreObstaclesInside();
        }
    }
}