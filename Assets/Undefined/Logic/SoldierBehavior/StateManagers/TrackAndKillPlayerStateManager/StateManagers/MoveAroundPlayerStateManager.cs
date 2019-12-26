using System;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace SoliderAIBehavior
{
    public struct MoveAroundPlayerStateManagerExternalCallbacks
    {
        public Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction;
        public Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction;

        public MoveAroundPlayerStateManagerExternalCallbacks(Func<IAgentMovementCalculationStrategy, NavMeshPathStatus> SetAIAgentDestinationAction,
            Action<AIMovementSpeedAttenuationFactor> SetAIAgentSpeedAttenuationAction)
        {
            this.SetAIAgentDestinationAction = SetAIAgentDestinationAction;
            this.SetAIAgentSpeedAttenuationAction = SetAIAgentSpeedAttenuationAction;
        }
    }

    /// <summary>
    /// The <see cref="TrackAndKillPlayerStateEnum.GO_ROUND_PLAYER"/> state is entered when the <see cref="SoliderEnemy"/> is
    /// trying to get the Player in sight by moving around the <see cref="PlayerObjectStateDataSystem.LastPlayerSeenPosition"/>.
    /// </summary>
    class MoveAroundPlayerStateManager : SoldierStateManager
    {
        /// <summary>
        /// When the SightDirection has been found <see cref="ComputeSightDirectionForTheQueriedDirection"/>, the final SightDirection orientation
        /// is offsted by this constant. This is to avoid the Agent to move around the player and stop at the exact line of sight to the Player.
        /// Stopping at the exact line of sight to the Player may cause that when the <see cref="TrackAndKillPlayerStateBehavior"/> switches state
        /// to <see cref="TrackAndKillPlayerStateEnum.SHOOTING_AT_PLAYER"/>, the projectile volume is not entering in contact with an obstacle.
        /// </summary>
        private const float DeltaAngleWhenSightDirectionIsComputed = 20f;

        private TrackAndKillPlayerStateBehavior _trackAndKillPlayerStateBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private MoveAroundPlayerStateManagerExternalCallbacks MoveAroundPlayerStateManagerExternalCallbacks;

        /// <summary>
        /// GameObject created on the fly that is used as a looking target for the <see cref="LookingAtAgentMovementCalculationStrategy"/>.
        /// /!\ It must be detroyed when <see cref="OnStateExit"/> is called.
        /// </summary>
        private GameObject TmpLastPlayerSeenPositionGameObject;


        public MoveAroundPlayerStateManager(TrackAndKillPlayerStateBehavior trackAndKillPlayerStateBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem, CoreInteractiveObject associatedInteractiveObject,
            WeaponFiringAreaSystem WeaponFiringAreaSystem,
            MoveAroundPlayerStateManagerExternalCallbacks MoveAroundPlayerStateManagerExternalCallbacks)
        {
            _trackAndKillPlayerStateBehaviorRef = trackAndKillPlayerStateBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            this.MoveAroundPlayerStateManagerExternalCallbacks = MoveAroundPlayerStateManagerExternalCallbacks;
            this.WeaponFiringAreaSystem = WeaponFiringAreaSystem;
        }

        public override void OnStateEnter()
        {
            var LastPlayerSeenPosition = this.PlayerObjectStateDataSystem.LastPlayerSeenPosition;
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
                this.MoveAroundPlayerStateManagerExternalCallbacks.SetAIAgentSpeedAttenuationAction.Invoke(AIMovementSpeedAttenuationFactor.RUN);
                if (this.MoveAroundPlayerStateManagerExternalCallbacks.SetAIAgentDestinationAction.Invoke(new LookingAtAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = LastPlayerSeenPosition.WorldPosition + SightDirection}, this.TmpLastPlayerSeenPositionGameObject.transform)) == NavMeshPathStatus.PathInvalid)
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
        /// /!\ The retained valid sight direction must be the closests possible than the direction AI -> LSP.
        /// "Positive" means when the sample angle is positive.
        /// </summary>
        /// <param name="AItoLVPDistance">The distance between <see cref="SoliderEnemy"/> and the Player.</param>
        /// <param name="SightDirection">The direction that will be used to set the next destination of the <see cref="SoliderEnemy"/>.</param>
        private static bool ComputeSightDirection(Vector3 AItoLVPDistance, Vector3 LastPlayerSeenPosition, out Vector3 SightDirection)
        {
            SightDirection = Vector3.zero;

            Vector3 PositiveSightDirection = Vector3.zero;
            Vector3 NeagtiveSightDirection = Vector3.zero;

            int positiveRaySuccessfulSample = 0;
            int negativeRaySuccessfulSample = 0;

            for (var SampleNumber = 1; SampleNumber <= 5; SampleNumber++)
            {
                /// We cast rays in the two opposite directions.
                /// This is to not discard the potential next obstacles intersection that will come with future samples. 
                bool positiveRay = ComputeSightDirectionForTheQueriedDirection(AItoLVPDistance, LastPlayerSeenPosition, SampleNumber * 10, ref PositiveSightDirection);
                bool negativeRay = ComputeSightDirectionForTheQueriedDirection(AItoLVPDistance, LastPlayerSeenPosition, -1 * SampleNumber * 10, ref NeagtiveSightDirection);

                if (positiveRay)
                {
                    positiveRaySuccessfulSample += 1;
                }

                if (negativeRay)
                {
                    negativeRaySuccessfulSample += 1;
                }

                /// When positive and negative samples are not the same, this means that the AI has a line of sight on one of them.
                if (positiveRay != negativeRay)
                {
                    if (positiveRay)
                    {
                        SightDirection = PositiveSightDirection;
                        return true;
                    }
                    else if (negativeRay)
                    {
                        SightDirection = NeagtiveSightDirection;
                        return true;
                    }
                }
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