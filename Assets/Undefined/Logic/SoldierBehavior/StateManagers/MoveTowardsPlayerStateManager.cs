﻿using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    /// <summary>
    /// Moves the <see cref="SoliderEnemy"/> to the position of the <see cref="InteractiveObjectTag.IsPlayer"/> object.
    /// (<see cref="SoldierAIStateEnum.MOVE_TOWARDS_PLAYER"/>)
    /// </summary>
    class MoveTowardsPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;
        private Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> SetDestinationAction;

        public MoveTowardsPlayerStateManager(SoldierAIBehavior soldierAiBehaviorRef, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            CoreInteractiveObject AssociatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem, WeaponFiringAreaSystem WeaponFiringAreaSystem, Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> destinationAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            this.WeaponFiringAreaSystem = WeaponFiringAreaSystem;
            SetDestinationAction = destinationAction;
        }

        public override void Tick(float d)
        {
            this.SetDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = this.PlayerObjectStateDataSystem.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition}),
                AIMovementSpeedDefinition.RUN);
            this.SwitchToShootingAtPlayer();
        }

        private void SwitchToShootingAtPlayer()
        {
            if (
                SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState(this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem) &&
                Vector3.Distance(this.PlayerObjectStateDataSystem.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition, this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition)
                    <= this.SoldierAIBehaviorDefinition.MaxDistancePlayerCatchUp)
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to GO_ROUND_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
            }
        }

        /// <summary>
        /// When the PlayerObject has just been out of sight, the <see cref="SoldierAIBehavior"/> state moves to <see cref="SoldierAIStateEnum.GO_ROUND_PLAYER"/>
        /// if the PlayerObject is behind an <see cref="InteractiveObjectTag.IsObstacle"/> object.
        /// </summary>
        public override void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            if (SoldierAIBehaviorUtil.InteractiveObjectBeyondObstacle(this.PlayerObjectStateDataSystem.PlayerObject(), this.AssociatedInteractiveObject))
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to GO_ROUND_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.GO_ROUND_PLAYER);
            }
            else
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
            }
        }
    }
}