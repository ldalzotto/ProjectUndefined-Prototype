using System;
using System.Collections.Generic;
using AIObjects;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using PlayerObject;
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
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;

        public SoldierAIBehavior(CoreInteractiveObject AssociatedInteractiveObject,
            Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> destinationAction, Action ClearpathAction, Action AskToFireAFiredProjectileAction
        ) : base(SoldierAIStateEnum.MOVE_TOWARDS_PLAYER)
        {
            this.PlayerObjectStateDataSystem = new PlayerObjectStateDataSystem(this.OnPlayerObjectJustOnSight, this.OnPlayerObjectJustOutOfSight);
            this.StateManagersLookup = new Dictionary<SoldierAIStateEnum, SoldierStateManager>()
            {
                {SoldierAIStateEnum.MOVE_TOWARDS_PLAYER, new MoveTowardsPlayerStateManager(this, this.PlayerObjectStateDataSystem, destinationAction)},
                {SoldierAIStateEnum.SHOOTING_AT_PLAYER, new ShootingAtPlayerStateManager(this, this.PlayerObjectStateDataSystem, AssociatedInteractiveObject, ClearpathAction, AskToFireAFiredProjectileAction)},
                {SoldierAIStateEnum.GO_ROUND_PLAYER, new MoveAroundPlayerStateManager(this, this.PlayerObjectStateDataSystem, AssociatedInteractiveObject, destinationAction)}
            };
        }

        public override void Tick(float d)
        {
            this.PlayerObjectStateDataSystem.Tick(d);
            base.Tick(d);
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
        private bool IsPlayerInSight;
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
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> SetDestinationAction;

        public MoveTowardsPlayerStateManager(SoldierAIBehavior soldierAiBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem, Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> destinationAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            SetDestinationAction = destinationAction;
        }

        public override void Tick(float d)
        {
            this.SetDestinationAction.Invoke(new ForwardAgentMovementCalculationStrategy(new AIDestination() {WorldPosition = this.PlayerObjectStateDataSystem.PlayerObject.InteractiveGameObject.GetTransform().WorldPosition}),
                AIMovementSpeedDefinition.RUN);
        }

        public override void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
        }
    }

    class ShootingAtPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;

        private Action ClearPathAction;
        private Action AskToFireAFiredProjectileAction;

        public ShootingAtPlayerStateManager(SoldierAIBehavior SoldierAIBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem,
            CoreInteractiveObject associatedInteractiveObject, Action clearPathAction, Action askToFireAFiredProjectileAction)
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
            this.AskToFireAFiredProjectileAction.Invoke();
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

    class MoveAroundPlayerStateManager : SoldierStateManager
    {
        private SoldierAIBehavior SoldierAIBehaviorRef;
        private PlayerObjectStateDataSystem PlayerObjectStateDataSystem;
        private CoreInteractiveObject AssociatedInteractiveObject;

        private Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> SetDestinationAction;

        private GameObject TmpLastPlayerSeenPositionGameObject;

        public MoveAroundPlayerStateManager(SoldierAIBehavior soldierAiBehaviorRef, PlayerObjectStateDataSystem playerObjectStateDataSystem, CoreInteractiveObject associatedInteractiveObject, Action<IAgentMovementCalculationStrategy, AIMovementSpeedDefinition> destinationAction)
        {
            SoldierAIBehaviorRef = soldierAiBehaviorRef;
            PlayerObjectStateDataSystem = playerObjectStateDataSystem;
            AssociatedInteractiveObject = associatedInteractiveObject;
            SetDestinationAction = destinationAction;
        }

        public override void OnStateEnter()
        {
            var LastPlayerSeenPosition = this.PlayerObjectStateDataSystem.LastPlayerSeenPosition;
            var AItoLVPDistance = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition - LastPlayerSeenPosition;

            bool SightDirectionFound = false;
            Vector3 SightDirection = Vector3.zero;
            for (var SampleNUmber = 1; SampleNUmber <= 5; SampleNUmber++)
            {
                var QueriedDirection = Quaternion.Euler(0, SampleNUmber * 10, 0) * AItoLVPDistance;
                if (!Physics.Raycast(LastPlayerSeenPosition, QueriedDirection.normalized, QueriedDirection.magnitude, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES)))
                {
                    SightDirection = QueriedDirection;
                    SightDirectionFound = true;
                    break;
                }

                QueriedDirection = Quaternion.Euler(0, -1 * SampleNUmber * 10, 0) * AItoLVPDistance;
                if (!Physics.Raycast(LastPlayerSeenPosition, QueriedDirection.normalized, QueriedDirection.magnitude, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_OBSTACLES)))
                {
                    SightDirection = QueriedDirection;
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

        public override void OnPlayerObjectJustOnSight(CoreInteractiveObject InSightInteractiveObject)
        {
            this.SoldierAIBehaviorRef.SetState(SoldierAIStateEnum.SHOOTING_AT_PLAYER);
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
        }
    }
}