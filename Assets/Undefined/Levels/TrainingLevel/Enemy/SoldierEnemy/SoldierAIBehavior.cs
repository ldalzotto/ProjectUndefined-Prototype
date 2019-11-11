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

    public static class SoldierAIBehaviorUtil
    {
        public static bool InteractiveObjectBeyondObstacle(CoreInteractiveObject InteractiveObject, CoreInteractiveObject SoliderEnemy)
        {
            var NotInSightInteractiveObjectWorldPos = InteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
            var AssociatedInteractiveobjectWorldPos = SoliderEnemy.InteractiveGameObject.GetTransform().WorldPosition;
            float DistanceFromAssociatedInteractiveObject = Vector3.Distance(NotInSightInteractiveObjectWorldPos, AssociatedInteractiveobjectWorldPos);
            return Physics.Raycast(NotInSightInteractiveObjectWorldPos, (AssociatedInteractiveobjectWorldPos - NotInSightInteractiveObjectWorldPos).normalized, DistanceFromAssociatedInteractiveObject, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES));
        }

        public static bool IsAllowToMoveToShootingAtPlayerState(PlayerObjectStateDataSystem PlayerObjectStateDataSystem, WeaponFiringAreaSystem WeaponFiringAreaSystem)
        {
            return PlayerObjectStateDataSystem.IsPlayerInSight && !WeaponFiringAreaSystem.AreObstaclesInside();
        }
    }

    public abstract class SoldierStateManager : StateManager
    {
        public virtual void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
        }

        public virtual void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
        }

        public virtual void OnDestinationReached()
        {
        }
    }

    public class PlayerObjectStateDataSystem
    {
        private CoreInteractiveObject playerObject;

        public CoreInteractiveObject PlayerObject()
        {
            if (this.playerObject == null)
            {
                this.playerObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
            }

            return this.playerObject;
        }

        public Vector3 LastPlayerSeenPosition { get; private set; }
        public bool IsPlayerInSight { get; private set; }
        private SoldierAIBehavior SoldierAIBehaviorRef;

        private Action<CoreInteractiveObject> OnPlayerObjectJustOnSightAction;
        private Action<CoreInteractiveObject> OnPlayerObjectJustOutOfSightAction;

        public PlayerObjectStateDataSystem(Action<CoreInteractiveObject> OnPlayerObjectJustOnSightAction, Action<CoreInteractiveObject> OnPlayerObjectJustOutOfSightAction)
        {
            this.OnPlayerObjectJustOnSightAction = OnPlayerObjectJustOnSightAction;
            this.OnPlayerObjectJustOutOfSightAction = OnPlayerObjectJustOutOfSightAction;
        }

        public void Tick(float d)
        {
            if (this.IsPlayerInSight)
            {
                this.LastPlayerSeenPosition = this.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition;
            }
        }

        public void OnInteractiveObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            if (InSightInteractiveObject.InteractiveObjectTag.IsPlayer)
            {
                this.IsPlayerInSight = true;
                this.OnPlayerObjectJustOnSightAction.Invoke(InSightInteractiveObject);
            }
        }

        public void OnInteractiveObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            if (NotInSightInteractiveObject.InteractiveObjectTag.IsPlayer)
            {
                this.IsPlayerInSight = false;
                this.OnPlayerObjectJustOutOfSightAction.Invoke(NotInSightInteractiveObject);
            }
        }
    }

    class PatrollingStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehavior;
        private AIPatrolSystem AIPatrolSystem;

        public PatrollingStateManager(SoldierAIBehavior SoldierAIBehavior, CoreInteractiveObject AssociatedInteractiveObject, AIPatrolSystemDefinition AIPatrolSystemDefinition)
        {
            this.SoldierAIBehavior = SoldierAIBehavior;
            this.AIPatrolSystem = new AIPatrolSystem(AssociatedInteractiveObject, AIPatrolSystemDefinition);
        }

        public override void Tick(float d)
        {
            this.AIPatrolSystem.Tick(d);
        }

        public override void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            Debug.Log(MyLog.Format("PatrollingStateManager to MOVE_TOWARDS_PLAYER"));
            this.SoldierAIBehavior.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
        }

        public override void OnDestinationReached()
        {
            this.AIPatrolSystem.OnAIDestinationReached();
        }
    }

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
            if (
                SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState(this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem) &&
                Vector3.Distance(this.PlayerObjectStateDataSystem.PlayerObject().InteractiveGameObject.GetTransform().WorldPosition, this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition) <= this.SoldierAIBehaviorDefinition.MaxDistancePlayerCatchUp)
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to GO_ROUND_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
            }
        }

        public override void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            if (SoldierAIBehaviorUtil.InteractiveObjectBeyondObstacle(this.PlayerObjectStateDataSystem.PlayerObject(), this.AssociatedInteractiveObject))
            {
                Debug.Log(MyLog.Format("MoveTowardsPlayerStateManager to GO_ROUND_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.GO_ROUND_PLAYER);
            }
        }
    }

    class ShootingAtPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;

        private Action ClearPathAction;
        private Action<Vector3> AskToFireAFiredProjectileAction;

        public ShootingAtPlayerStateManager(SoldierAIBehavior SoldierAIBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            CoreInteractiveObject associatedInteractiveObject, Action clearPathAction, Action<Vector3> askToFireAFiredProjectileAction)
        {
            this.SoldierAIBehaviorRef = SoldierAIBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            ClearPathAction = clearPathAction;
            AskToFireAFiredProjectileAction = askToFireAFiredProjectileAction;
        }

        public override void OnStateEnter()
        {
            this.ClearPathAction.Invoke();
        }

        public override void Tick(float d)
        {
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject();
            this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation =
                Quaternion.LookRotation((PlayerObject.InteractiveGameObject.GetTransform().WorldPosition - this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition).normalized, Vector3.up);
            var WorldTargetDirection = ((PlayerObject.InteractiveGameObject.GetTransform().WorldPosition + PlayerObject.GetFiringTargetLocalPosition())
                                        - (this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + this.AssociatedInteractiveObject.GetFiringTargetLocalPosition())).normalized;
            this.AskToFireAFiredProjectileAction.Invoke(WorldTargetDirection);
        }

        public override void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            if (SoldierAIBehaviorUtil.InteractiveObjectBeyondObstacle(NotInSightInteractiveObject, this.AssociatedInteractiveObject))
            {
                Debug.Log(MyLog.Format("ShootingAtPlayerStateManager to GO_ROUND_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.GO_ROUND_PLAYER);
            }
            else
            {
                Debug.Log(MyLog.Format("ShootingAtPlayerStateManager to MOVE_TOWARDS_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
            }
        }
    }

    public class WeaponFiringAreaSystem : AInteractiveObjectPhysicsEventListener
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private BoxRangeObjectV2 WeaponFiringAreaBoxRangeObject;
        private Func<Vector3> GetWeaponFirePointOriginLocalAction;

        private List<Collider> InsideWeaponFiringAreaObstacles = new List<Collider>();

        public WeaponFiringAreaSystem(CoreInteractiveObject associatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem, Func<Vector3> weaponFirePointOriginLocalAction)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            GetWeaponFirePointOriginLocalAction = weaponFirePointOriginLocalAction;
            this.WeaponFiringAreaBoxRangeObject = new BoxRangeObjectV2(associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new BoxRangeObjectInitialization()
            {
                RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                IsTakingIntoAccountObstacles = false,
                BoxRangeTypeDefinition = new BoxRangeTypeDefinition()
            }, associatedInteractiveObject, "WeaponFiringAreaBoxRangeObject");
            this.WeaponFiringAreaBoxRangeObject.RegisterPhysicsEventListener(this);
        }

        public void Tick(float d)
        {
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.localPosition = this.GetWeaponFirePointOriginLocalAction.Invoke();
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject();
            var PlayerObjectWorldPosition = PlayerObject.InteractiveGameObject.GetTransform().WorldPosition;
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.rotation = Quaternion.LookRotation((PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition() - this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition).normalized);

            var DistanceSoldierPlayer = Vector3.Distance((PlayerObjectWorldPosition + PlayerObject.GetFiringTargetLocalPosition()), this.WeaponFiringAreaBoxRangeObject.GetTransform().WorldPosition);
            this.WeaponFiringAreaBoxRangeObject.SetLocalCenter(new Vector3(0, 0, DistanceSoldierPlayer * 0.5f));
            this.WeaponFiringAreaBoxRangeObject.SetLocalSize(new Vector3(2, 2, DistanceSoldierPlayer));
        }

        public bool AreObstaclesInside()
        {
            return this.InsideWeaponFiringAreaObstacles.Count > 0;
        }

        public override bool ColliderSelectionGuard(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo)
        {
            return interactiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsObstacle;
        }

        public override void OnTriggerEnter(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.InsideWeaponFiringAreaObstacles.Add(PhysicsTriggerInfo.Other);
        }

        public override void OnTriggerExit(InteractiveObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.InsideWeaponFiringAreaObstacles.Remove(PhysicsTriggerInfo.Other);
        }

        public void Destroy()
        {
            this.WeaponFiringAreaBoxRangeObject.OnDestroy();
            GameObject.Destroy(this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject);
        }
    }

    class MoveAroundPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private WeaponFiringAreaSystem WeaponFiringAreaSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private Func<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition, NavMeshPathStatus> SetDestinationAction;

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

            bool SightDirectionFound = false;
            Vector3 SightDirection = Vector3.zero;
            for (var SampleNumber = 1; SampleNumber <= 5; SampleNumber++)
            {
                var RotationAngle = SampleNumber * 10;
                var QueriedDirection = Quaternion.Euler(0, RotationAngle, 0) * AItoLVPDistance;
                if (!Physics.Raycast(LastPlayerSeenPosition, QueriedDirection.normalized, QueriedDirection.magnitude, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES)))
                {
                    SightDirection = Quaternion.Euler(0, Math.Sign(RotationAngle) * 50, 0) * AItoLVPDistance;
                    SightDirectionFound = true;
                    break;
                }

                RotationAngle = -1 * SampleNumber * 10;
                QueriedDirection = Quaternion.Euler(0, RotationAngle, 0) * AItoLVPDistance;
                if (!Physics.Raycast(LastPlayerSeenPosition, QueriedDirection.normalized, QueriedDirection.magnitude, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES)))
                {
                    SightDirection = Quaternion.Euler(0, Math.Sign(RotationAngle) * 50, 0) * AItoLVPDistance;
                    SightDirectionFound = true;
                    break;
                }
            }

            if (SightDirectionFound)
            {
                this.TmpLastPlayerSeenPositionGameObject = new GameObject("TmpLastPlayerSeenPositionGameObject");
                this.TmpLastPlayerSeenPositionGameObject.transform.position = LastPlayerSeenPosition;
                if (this.SetDestinationAction.Invoke(new LookingAtAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = LastPlayerSeenPosition + SightDirection}, this.TmpLastPlayerSeenPositionGameObject.transform),
                        AIMovementSpeedDefinition.RUN) == NavMeshPathStatus.PathInvalid)
                {
                    Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TOWARDS_PLAYER"));
                    this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
                }
            }
            else
            {
                Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to MOVE_TOWARDS_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
            }
        }

        public override void Tick(float d)
        {
            if (SoldierAIBehaviorUtil.IsAllowToMoveToShootingAtPlayerState(this.PlayerObjectStateDataSystem, this.WeaponFiringAreaSystem))
            {
                Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to SHOOTING_AT_PLAYER"));
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
            }
        }

        public override void OnDestinationReached()
        {
            Debug.Log(MyLog.Format("MoveAroundPlayerStateManager to PATROLLING"));
            this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.PATROLLING);
        }

        public override void OnStateExit()
        {
            if (this.TmpLastPlayerSeenPositionGameObject != null)
            {
                GameObject.Destroy(this.TmpLastPlayerSeenPositionGameObject.gameObject);
            }
        }
    }
}