using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
using RangeObjects;
using UnityEngine;

namespace TrainingLevel
{
    public enum SoldierAIStateEnum
    {
        MOVE_TOWARDS_PLAYER = 0,
        GO_ROUND_PLAYER = 1,
        SHOOTING_AT_PLAYER = 2
    }

    public class SoldierAIBehavior : AIBehavior<SoldierAIStateEnum, SoldierStateManager>
    {
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;

        public SoldierAIBehavior(CoreInteractiveObject AssociatedInteractiveObject, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> destinationAction, Action ClearpathAction, Action<Vector3> AskToFireAFiredProjectileAction, Func<Vector3> GetWeaponFirePointOriginLocalAction
        ) : base(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER)
        {
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.PlayerObjectStateDataSystem = new PlayerObjectStateDataSystem(this.OnPlayerObjectJustOnSight, this.OnPlayerObjectJustOutOfSight);
            this.StateManagersLookup = new Dictionary<SoldierAIStateEnum, SoldierStateManager>()
            {
                {SoldierAIStateEnum.MOVE_TOWARDS_PLAYER, new MoveTowardsPlayerStateManager(this, SoldierAIBehaviorDefinition, AssociatedInteractiveObject, this.PlayerObjectStateDataSystem, destinationAction)},
                {SoldierAIStateEnum.SHOOTING_AT_PLAYER, new ShootingAtPlayerStateManager(this, this.PlayerObjectStateDataSystem, AssociatedInteractiveObject, ClearpathAction, AskToFireAFiredProjectileAction)},
                {SoldierAIStateEnum.GO_ROUND_PLAYER, new MoveAroundPlayerStateManager(this, this.PlayerObjectStateDataSystem, AssociatedInteractiveObject, destinationAction, GetWeaponFirePointOriginLocalAction)}
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

    class PlayerObjectStateDataSystem
    {
        public CoreInteractiveObject PlayerObject { get; private set; }
        public Vector3 LastPlayerSeenPosition { get; private set; }
        public bool IsPlayerInSight { get; private set; }
        private SoldierAIBehavior SoldierAIBehaviorRef;

        private Action<CoreInteractiveObject> OnPlayerObjectJustOnSightAction;
        private Action<CoreInteractiveObject> OnPlayerObjectJustOutOfSightAction;

        public PlayerObjectStateDataSystem(Action<CoreInteractiveObject> OnPlayerObjectJustOnSightAction, Action<CoreInteractiveObject> OnPlayerObjectJustOutOfSightAction)
        {
            this.OnPlayerObjectJustOnSightAction = OnPlayerObjectJustOnSightAction;
            this.OnPlayerObjectJustOutOfSightAction = OnPlayerObjectJustOutOfSightAction;
            this.PlayerObject = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject;
        }

        public void Tick(float d)
        {
            if (this.IsPlayerInSight)
            {
                this.LastPlayerSeenPosition = this.PlayerObject.InteractiveGameObject.GetTransform().WorldPosition;
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

    class MoveTowardsPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> SetDestinationAction;

        public MoveTowardsPlayerStateManager(SoldierAIBehavior soldierAiBehaviorRef, SoldierAIBehaviorDefinition SoldierAIBehaviorDefinition,
            CoreInteractiveObject AssociatedInteractiveObject, PlayerObjectStateDataSystem playerObjectStateDataSystem, Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> destinationAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            this.SoldierAIBehaviorDefinition = SoldierAIBehaviorDefinition;
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            SetDestinationAction = destinationAction;
        }

        public override void Tick(float d)
        {
            this.SetDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = this.PlayerObjectStateDataSystem.PlayerObject.InteractiveGameObject.GetTransform().WorldPosition}),
                AIMovementSpeedDefinition.RUN);
            if (
                this.PlayerObjectStateDataSystem.IsPlayerInSight &&
                Vector3.Distance(this.PlayerObjectStateDataSystem.PlayerObject.InteractiveGameObject.GetTransform().WorldPosition, this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition) <= this.SoldierAIBehaviorDefinition.MaxDistancePlayerCatchUp)
            {
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
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
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject;
            this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform.rotation =
                Quaternion.LookRotation((PlayerObject.InteractiveGameObject.GetTransform().WorldPosition - this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition).normalized, Vector3.up);
            var WorldTargetDirection = ((PlayerObject.InteractiveGameObject.GetTransform().WorldPosition + PlayerObject.GetFiringTargetLocalPosition())
                                        - (this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + this.AssociatedInteractiveObject.GetFiringTargetLocalPosition())).normalized;
            this.AskToFireAFiredProjectileAction.Invoke(WorldTargetDirection);
        }

        public override void OnPlayerObjectJustOutOfSight(CoreInteractiveObject NotInSightInteractiveObject)
        {
            var NotInSightInteractiveObjectWorldPos = NotInSightInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
            var AssociatedInteractiveobjectWorldPos = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition;
            float DistanceFromAssociatedInteractiveObject = Vector3.Distance(NotInSightInteractiveObjectWorldPos, AssociatedInteractiveobjectWorldPos);
            if (Physics.Raycast(NotInSightInteractiveObjectWorldPos, (AssociatedInteractiveobjectWorldPos - NotInSightInteractiveObjectWorldPos).normalized, DistanceFromAssociatedInteractiveObject, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES)))
            {
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.GO_ROUND_PLAYER);
            }
            else
            {
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
            }
        }
    }

    class WeaponFiringAreaSystem : AInteractiveObjectPhysicsEventListener
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
            this.Tick(0f);
        }

        public void Tick(float d)
        {
            this.WeaponFiringAreaBoxRangeObject.RangeGameObjectV2.RangeGameObject.transform.localPosition = this.GetWeaponFirePointOriginLocalAction.Invoke();
            var PlayerObject = this.PlayerObjectStateDataSystem.PlayerObject;
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
        private Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> SetDestinationAction;
        private Func<Vector3> WeaponFirePointOriginLocalAction;

        private GameObject TmpLastPlayerSeenPositionGameObject;

        public MoveAroundPlayerStateManager(SoldierAIBehavior soldierAiBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem, CoreInteractiveObject associatedInteractiveObject,
            Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> destinationAction, Func<Vector3> weaponFirePointOriginLocalAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            SetDestinationAction = destinationAction;
            this.WeaponFirePointOriginLocalAction = weaponFirePointOriginLocalAction;
        }

        public override void OnStateEnter()
        {
            this.WeaponFiringAreaSystem = new WeaponFiringAreaSystem(this.AssociatedInteractiveObject, this.PlayerObjectStateDataSystem, this.WeaponFirePointOriginLocalAction);
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
                this.SetDestinationAction.Invoke(new LookingAtAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = LastPlayerSeenPosition + SightDirection}, this.TmpLastPlayerSeenPositionGameObject.transform),
                    AIMovementSpeedDefinition.WALK);
            }
            else
            {
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
            }
        }

        public override void Tick(float d)
        {
            this.WeaponFiringAreaSystem.Tick(d);
            if (this.PlayerObjectStateDataSystem.IsPlayerInSight && !this.WeaponFiringAreaSystem.AreObstaclesInside())
            {
                this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
            }
        }

        public override void OnDestinationReached()
        {
            this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER);
        }

        public override void OnStateExit()
        {
            if (this.TmpLastPlayerSeenPositionGameObject != null)
            {
                GameObject.Destroy(this.TmpLastPlayerSeenPositionGameObject.gameObject);
            }

            if (this.WeaponFiringAreaSystem != null)
            {
                this.WeaponFiringAreaSystem.Destroy();
            }
        }
    }
}