using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace TrainingLevel
{
    /// <summary>
    /// The <see cref="SoldierAIStateEnum.GO_ROUND_PLAYER"/> state is entered when the <see cref="SoliderEnemy"/> is
    /// trying to get the Player in sight by moving around the <see cref="PlayerObjectStateDataSystem.LastPlayerSeenPosition"/>.
    /// </summary>
    class MoveAroundPlayerStateManager : SoldierStateManager
    {
        /// <summary>
        /// When the SightDirection has been found <see cref="ComputeSightDirectionForTheQueriedDirection"/>, the final SightDirection orientation
        /// is offsted by this constant. This is to avoid the Agent to move around the player and stop at the exact line of sight to the Player.
        /// Stopping at the exact line of sight to the Player may cause that when the <see cref="SoldierAIBehavior"/> switches state
        /// to <see cref="SoldierAIStateEnum.SHOOTING_AT_PLAYER"/>, the projectile volume is not entering in contact with an obstacle.
        /// </summary>
        private const float DeltaAngleWhenSightDirectionIsComputed = 20f;

        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> SetDestinationAction;

        /// <summary>
        /// GameObject created on the fly that is used as a looking target for the <see cref="LookingAtAgentMovementCalculationStrategy"/>.
        /// /!\ It must be detroyed when <see cref="OnStateExit"/> is called.
        /// </summary>
        private GameObject TmpLastPlayerSeenPositionGameObject;

        public MoveAroundPlayerStateManager(SoldierAIBehavior soldierAiBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem, CoreInteractiveObject associatedInteractiveObject,
            WeaponFiringAreaSystem WeaponFiringAreaSystem,
            Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> destinationAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            SetDestinationAction = destinationAction;
            this.WeaponFiringAreaSystem = WeaponFiringAreaSystem;
        }

        public override void OnStateEnter()
        {
            var LastPlayerSeenPosition = this.PlayerObjectStateDataSystem.LastPlayerSeenPosition;
            var AItoLVPDistance = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition - LastPlayerSeenPosition;

            /// If we found a SightDirection without Obstacle
            if (ComputeSightDirection(AItoLVPDistance, LastPlayerSeenPosition, out var SightDirection))
            {
                this.TmpLastPlayerSeenPositionGameObject = new GameObject("TmpLastPlayerSeenPositionGameObject");
                this.TmpLastPlayerSeenPositionGameObject.transform.position = LastPlayerSeenPosition;
                /// Setting the Agent destination and always facing the TmpLastPlayerSeenPositionGameObject
                if (this.SetDestinationAction.Invoke(new LookingAtAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = LastPlayerSeenPosition + SightDirection}, this.TmpLastPlayerSeenPositionGameObject.transform),
                        AIMovementSpeedDefinition.RUN) == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                    this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
                }
            }
            else
            {
                Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
            }
        }


        public override void Tick(float d)
        {
            //If the PlayerObject is seen and can shoot.
            if (SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState(this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem))
            {
                Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to SHOOTING_AT_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
            }
        }

        /// <summary>
        /// If this method is called, this means that the <see cref="SoldierAIBehavior"/> haven't seen the Player after it's movement.
        /// We consider that the Player is lost -> switch to <see cref="SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION"/>.
        /// </summary>
        public override void OnDestinationReached()
        {
            Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
            this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
        }

        public override void OnStateExit()
        {
            this.OnDestroy();
        }

        public override void OnDestroy()
        {
            if (this.TmpLastPlayerSeenPositionGameObject != null)
            {
                GameObject.Destroy(this.TmpLastPlayerSeenPositionGameObject.gameObject);
            }
        }

        #region Sight Direction calculation

        /// <summary>
        /// SightDirection computation is done by sampling different directions from the Player and checking if
        /// there is Obstacles in this direction.
        /// When a direction with no Obstacle is found, then the final direction is offsetter by <see cref="DeltaAngleWhenSightDirectionIsComputed"/>.
        /// </summary>
        /// <param name="AItoLVPDistance">The distance between <see cref="SoliderEnemy"/> and the Player.</param>
        /// <param name="SightDirection">The direction that will be used to set the next destination of the <see cref="SoliderEnemy"/>.</param>
        /// <returns></returns>
        private static bool ComputeSightDirection(Vector3 AItoLVPDistance, Vector3 LastPlayerSeenPosition, out Vector3 SightDirection)
        {
            SightDirection = Vector3.zero;
            for (var SampleNumber = 1; SampleNumber <= 5; SampleNumber++)
            {
                if (ComputeSightDirectionForTheQueriedDirection(AItoLVPDistance, LastPlayerSeenPosition, SampleNumber * 10, ref SightDirection))
                    return true;

                if (ComputeSightDirectionForTheQueriedDirection(AItoLVPDistance, LastPlayerSeenPosition, -1 * SampleNumber * 10, ref SightDirection))
                    return true;
            }

            return false;
        }

        /// <param name="AItoLVPDistance">The distance between <see cref="SoliderEnemy"/> and the Player.</param>
        private static bool ComputeSightDirectionForTheQueriedDirection(Vector3 AItoLVPDistance, Vector3 LastPlayerSeenPosition, float RotationAngle, ref Vector3 SightDirection)
        {
            /// The Queried direction is projected on the (X,Z) plane.
            var QueriedDirection = Quaternion.Euler(0, RotationAngle, 0) * AItoLVPDistance;

            /// If there is no Obstacles in for the QueriedDirection
            if (!RaycastFromLastPlayerSeenPosition(LastPlayerSeenPosition, QueriedDirection))
            {
                /// The SightDrection is slightly offsetted by DeltaAngleWhenSightDirectionIsComputed in the direction of the RotationAngle
                SightDirection = Quaternion.Euler(0, RotationAngle + (Math.Sign(RotationAngle) * DeltaAngleWhenSightDirectionIsComputed), 0) * AItoLVPDistance;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs a <see cref="Physics.Raycast"/> on the <see cref="LayerConstants.PUZZLE_OBSTACLES"/> layer only.
        /// </summary>
        /// <param name="LastPlayerSeenPosition">The origin of the raycast.</param>
        /// <param name="QueriedDirection">The direction of the raycast</param>
        private static bool RaycastFromLastPlayerSeenPosition(Vector3 LastPlayerSeenPosition, Vector3 QueriedDirection)
        {
            return Physics.Raycast(LastPlayerSeenPosition, QueriedDirection.normalized, QueriedDirection.magnitude, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES));
        }

        #endregion
    }
}