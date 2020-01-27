using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    /// <summary>
    /// The <see cref="TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER"/> state is entered when the <see cref="SoliderEnemy"/> is
    /// trying to get the Player in sight by moving around the <see cref="PlayerObjectStateDataSystem.LastPlayerSeenPosition"/>.
    /// </summary>
    class MoveAroundPlayerStateManager : SoldierStateManager
    {
        /// <summary>
        /// When the SightDirection has been found, the final SightDirection orientation
        /// is offsted by this factor <see cref="OffsetFoundedSightDirection"/>. This is to avoid the Agent to move around the player and stop at the exact line of sight to the Player.
        /// Stopping at the exact line of sight to the Player may cause that when the <see cref="TrackAndKillPlayerStateBehavior"/> switches state
        /// to <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>, the projectile volume is not entering in contact with an obstacle.
        /// </summary>
        private const float DeltaAngleWhenSightDirectionIsComputedInPercentage = 0.05f;

        private TrackAndKillPlayerStateBehavior _trackAndKillPlayerStateBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private ISetAIAgentDestinationActionCallback ISetAIAgentDestinationActionCallback;
        private IMoveAroundPlayerStateManagerWorkflowCallback IMoveAroundPlayerStateManagerWorkflowCallback;

        /// <summary>
        /// GameObject created on the fly that is used as a looking target for the <see cref="LookingAtAgentMovementCalculationStrategy"/>.
        /// /!\ It must be detroyed when <see cref="OnStateExit"/> is called.
        /// </summary>
        private GameObject TmpLastPlayerSeenPositionGameObject;

        private LookingAtAgentMovementCalculationStrategy CalculatedLookingAtAgentMovementCalculationStrategy;

        public MoveAroundPlayerStateManager(TrackAndKillPlayerStateBehavior trackAndKillPlayerStateBehaviorRef,
            PlayerObjectStateDataSystem playerObjectStateDataSystem, CoreInteractiveObject associatedInteractiveObject,
            WeaponFiringAreaSystem WeaponFiringAreaSystem,
            ISetAIAgentDestinationActionCallback ISetAIAgentDestinationActionCallback, 
            IMoveAroundPlayerStateManagerWorkflowCallback IMoveAroundPlayerStateManagerWorkflowCallback)
        {
            _trackAndKillPlayerStateBehaviorRef = trackAndKillPlayerStateBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.ISetAIAgentDestinationActionCallback = ISetAIAgentDestinationActionCallback;
            this.IMoveAroundPlayerStateManagerWorkflowCallback = IMoveAroundPlayerStateManagerWorkflowCallback;
            this.WeaponFiringAreaSystem = WeaponFiringAreaSystem;
        }

        public override void OnStateEnter()
        {   
            var LastPlayerSeenPosition = this.PlayerObjectStateDataSystem.LastPlayerSeenPosition;
            this.IMoveAroundPlayerStateManagerWorkflowCallback?.OnMoveAroundPlayerStartedAction.Invoke(LastPlayerSeenPosition.WorldPosition);
            
            var AItoLVPDistance = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition - LastPlayerSeenPosition.WorldPosition;

            /// The last player seen position is offsetted by the AI->LPS to avoid borderline case where the Player is behind an obstacles
            /// but ComputeSightDirection doesn't return intersection because the Player is just at the limit of the obstacle.
            var offsettedLastPlayerSeenPosition = LastPlayerSeenPosition.WorldPosition +
                                                  (LastPlayerSeenPosition.WorldPosition - this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition).normalized;

            /// If we found a SightDirection without Obstacle
            if (ComputeSightDirection(AItoLVPDistance, offsettedLastPlayerSeenPosition, out var SightDirection))
            {
                this.TmpLastPlayerSeenPositionGameObject = new GameObject("TmpLastPlayerSeenPositionGameObject");
                this.TmpLastPlayerSeenPositionGameObject.transform.position = LastPlayerSeenPosition.WorldPosition;

                /// Setting the Agent destination and always facing the TmpLastPlayerSeenPositionGameObject
                this.ISetAIAgentDestinationActionCallback.SetAIAgentSpeedAttenuationAction.Invoke(AIMovementSpeedAttenuationFactor.RUN);
                this.CalculatedLookingAtAgentMovementCalculationStrategy = new LookingAtAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = LastPlayerSeenPosition.WorldPosition + SightDirection}, this.TmpLastPlayerSeenPositionGameObject.transform);
                if (this.ISetAIAgentDestinationActionCallback.SetAIAgentDestinationAction.Invoke(this.CalculatedLookingAtAgentMovementCalculationStrategy) == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                    this._trackAndKillPlayerStateBehaviorRef.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
                }
            }
            else
            {
                Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
                this._trackAndKillPlayerStateBehaviorRef.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
            }
        }


        public override void Tick(float d)
        {
            //If the PlayerObject is seen and can shoot.
            if (SoldierAIBehaviorUtil.PlayerInSightButNoObstaclesBetween(this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem))
            {
                Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to SHOOTING_AT_PLAYER"));
                this._trackAndKillPlayerStateBehaviorRef.SetState(TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER);
            }
            /// Else, we keep moving to the destination
            /// Destination is cached, so no calculation every frame
            else
            {
                this.ISetAIAgentDestinationActionCallback.SetAIAgentSpeedAttenuationAction.Invoke(AIMovementSpeedAttenuationFactor.RUN);
                this.ISetAIAgentDestinationActionCallback.SetAIAgentDestinationAction.Invoke(this.CalculatedLookingAtAgentMovementCalculationStrategy);
            }
        }

        /// <summary>
        /// If this method is called, this means that the <see cref="TrackAndKillPlayerStateBehavior"/> haven't seen the Player after it's movement.
        /// We consider that the Player is lost -> switch to <see cref="TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION"/>.
        /// </summary>
        public override void OnDestinationReached()
        {
            Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TO_LAST_SEEN_PLAYER_POSITION"));
            this._trackAndKillPlayerStateBehaviorRef.SetState(TrackAndKillPlayerStateEnum.MOVE_TO_LAST_SEEN_PLAYER_POSITION);
        }

        public override void OnStateExit()
        {
            this.OnDestroy();
            this.IMoveAroundPlayerStateManagerWorkflowCallback?.OnMoveAroundPlayerEndedAction();
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
        /// The final direction is offsettet by <see cref="DeltaAngleWhenSightDirectionIsComputed"/>.
        /// /!\ The retained valid sight direction must be the closest possible than the direction AI -> LSP.
        /// "Positive" means when the sample angle is positive.
        /// </summary>
        /// <param name="AItoLVPDistance">The distance between <see cref="SoliderEnemy"/> and the Player.</param>
        /// <param name="SightDirection">The direction that will be used to set the next destination of the <see cref="SoliderEnemy"/>.</param>
        private static bool ComputeSightDirection(Vector3 AItoLVPDistance, Vector3 LastPlayerSeenPosition, out Vector3 SightDirection)
        {
            SightDirection = Vector3.zero;

            for (var SampleNumber = 1; SampleNumber <= 9; SampleNumber++)
            {
                /// We cast rays in the two opposite directions.
                /// This is to not discard the potential next obstacles intersection that will come with future samples. 
                float positiveRotationAngleInDegree = SampleNumber * 10;

                bool positiveAngleObstructedByObstacles = CheckIfThereIsObstacles(AItoLVPDistance, LastPlayerSeenPosition, positiveRotationAngleInDegree);
                bool negativeAngleObstructedByObstacles = CheckIfThereIsObstacles(AItoLVPDistance, LastPlayerSeenPosition, -positiveRotationAngleInDegree);

                /// When positive and negative samples are not the same, this means that the AI has a line of sight on one of them.
                if (positiveAngleObstructedByObstacles != negativeAngleObstructedByObstacles)
                {
                    /// If the positive angle is not obstructed
                    if (!positiveAngleObstructedByObstacles)
                    {
                        SightDirection = OffsetFoundedSightDirection(AItoLVPDistance, LastPlayerSeenPosition, positiveRotationAngleInDegree);
                        return true;
                    }
                    /// If the negative angle is not obstructed
                    else if (!negativeAngleObstructedByObstacles)
                    {
                        SightDirection = OffsetFoundedSightDirection(AItoLVPDistance, LastPlayerSeenPosition, -positiveRotationAngleInDegree);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the offsetted SightDirection according to the <see cref="DeltaAngleWhenSightDirectionIsComputedInPercentage"/> factor.
        /// When the offsetted SightDirection is calculated, an ObstacleOcclusion check is performed (<see cref="CheckIfThereIsObstacles"/>).
        /// If the offstted SightDirection is occluded by an obstacle, this means that the final AI position and direction will be occluded to shoot.
        ///    -> If this case, we fallback to initial SightDirection which has already been ensured that it is not occluded. 
        /// </summary>
        private static Vector3 OffsetFoundedSightDirection(Vector3 AItoLVPDistance, Vector3 LastPlayerSeenPosition, float initialAngle)
        {
            var offsettedAngle = initialAngle + (initialAngle * DeltaAngleWhenSightDirectionIsComputedInPercentage);
            if (!CheckIfThereIsObstacles(AItoLVPDistance, LastPlayerSeenPosition, offsettedAngle))
            {
                return Quaternion.Euler(0, offsettedAngle, 0) * AItoLVPDistance;
            }
            /// the FALLBACK as described in the method description
            else
            {
                return Quaternion.Euler(0, initialAngle, 0) * AItoLVPDistance;
            }
        }

        private static bool CheckIfThereIsObstacles(Vector3 AItoLVPDistance, Vector3 LastPlayerSeenPosition, float RotationAngle)
        {
            /// The Queried direction is projected on the (X,Z) plane.
            var QueriedDirection = Quaternion.Euler(0, RotationAngle, 0) * AItoLVPDistance;

            /// If there is Obstacles in for the QueriedDirection
            return RaycastFromLastPlayerSeenPosition(LastPlayerSeenPosition, QueriedDirection);
        }

        /// <summary>
        /// Performs a <see cref="Physics.Raycast"/> on the <see cref="LayerConstants.PUZZLE_OBSTACLES"/> layer only.
        /// </summary>
        /// <param name="LastPlayerSeenPosition">The origin of the raycast.</param>
        /// <param name="QueriedDirection">The direction of the raycast</param>
        private static bool RaycastFromLastPlayerSeenPosition(Vector3 LastPlayerSeenPosition, Vector3 QueriedDirection)
        {
            bool raycastResult = Physics.Raycast(LastPlayerSeenPosition, QueriedDirection.normalized, out RaycastHit hit, QueriedDirection.magnitude, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES));

#if UNITY_EDITOR
            if (raycastResult)
            {
                Debug.DrawLine(LastPlayerSeenPosition, LastPlayerSeenPosition + QueriedDirection, Color.red);
            }
            else
            {
                Debug.DrawLine(LastPlayerSeenPosition, LastPlayerSeenPosition + QueriedDirection, Color.green);
            }
#endif

            return raycastResult;
        }

        #endregion
    }
}