using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using RangeObjects;
using UnityEngine;
using UnityEngine.AI;

namespace TrainingLevel
{
    public enum SoldierAIStateEnum
    {
        MOVE_TOWARDS_PLAYER = 0,
        GO_ROUND_PLAYER = 1,

        /// <summary>
        /// /!\ Switching to this state is only allowed if <see cref="SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState"/> conditions are fulfilled.
        /// </summary>
        SHOOTING_AT_PLAYER = 2,
        PATROLLING = 3
    }

    public class SoldierAIBehavior : AIBehavior<SoldierAIStateEnum, SoldierStateManager>
    {
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;

        public SoldierAIBehavior(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> destinationAction, Action ClearpathAction, Action<Vector3> AskToFireAFiredProjectileAction, Func<Vector3> GetWeaponFirePointOriginLocalAction
        ) : base(SoldierAIStateEnum.PATROLLING)
        {
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.PlayerObjectStateDataSystem = new PlayerObjectStateDataSystem(this.OnPlayerObjectJustOnSight, this.OnPlayerObjectJustOutOfSight);
            this.WeaponFiringAreaSystem = new WeaponFiringAreaSystem(AssociatedInteractiveObject, this.PlayerObjectStateDataSystem, GetWeaponFirePointOriginLocalAction);
            this.StateManagersLookup = new Dictionary<SoldierAIStateEnum, SoldierStateManager>()
            {
                {SoldierAIStateEnum.PATROLLING, new PatrollingStateManager(this, AssociatedInteractiveObject, SoldierAIBehaviorDefinition.AIPatrolSystemDefinition)},
                {SoldierAIStateEnum.MOVE_TOWARDS_PLAYER, new MoveTowardsPlayerStateManager(this, SoldierAIBehaviorDefinition, AssociatedInteractiveObject, this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem, destinationAction)},
                {SoldierAIStateEnum.SHOOTING_AT_PLAYER, new ShootingAtPlayerStateManager(this, this.PlayerObjectStateDataSystem, AssociatedInteractiveObject, ClearpathAction, AskToFireAFiredProjectileAction)},
                {SoldierAIStateEnum.GO_ROUND_PLAYER, new MoveAroundPlayerStateManager(this, this.PlayerObjectStateDataSystem, AssociatedInteractiveObject, this.WeaponFiringAreaSystem, destinationAction)}
            };
        }

        public override void Tick(float d)
        {
            this.PlayerObjectStateDataSystem.Tick(d);
            this.WeaponFiringAreaSystem.Tick(d);
            base.Tick(d);
        }

        public override void SetState(SoldierAIStateEnum NewState)
        {
            base.SetState(NewState);
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
            this.GetCurrentStateManager().OnPlayerObjectJustOnSight(InSightInteractiveObject);
        }

        private void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            this.GetCurrentStateManager().OnPlayerObjectJustOutOfSight(NotInSightInteractiveObject);
        }

        #endregion

        #region External Agent Events

        public void OnDestinationReached()
        {
            this.GetCurrentStateManager().OnDestinationReached();
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