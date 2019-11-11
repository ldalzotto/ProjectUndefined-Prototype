using AIObjects;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class AggressiveAIObject : CoreInteractiveObject
    {
        [VE_Nested] private AIPatrollingState AIPatrollingState;

        private AIPatrolSystem AIPatrolSystem;

        [VE_Nested] private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        [DrawNested] private SightObjectSystem SightObjectSystem;
        private AggressiveObjectInitializerData AggressiveObjectInitializerData;
        [VE_Nested] private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystem;

        public AggressiveAIObject(IInteractiveGameObject interactiveGameObject, AggressiveObjectInitializerData AIInteractiveObjectInitializerData)
        {
            this.AggressiveObjectInitializerData = AIInteractiveObjectInitializerData;
            interactiveGameObject.CreateLogicCollider(AIInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            interactiveGameObject.CreateAgent(AIInteractiveObjectInitializerData.AIAgentDefinition);
            base.BaseInit(interactiveGameObject, true);
        }

        public override void Init()
        {
            interactiveObjectTag = new InteractiveObjectTag {IsAi = true};
            AIPatrollingState = new AIPatrollingState();
            AIPatrollingState.isPatrolling = true;
            AIPatrolSystem = new AIPatrolSystem(this, this.AggressiveObjectInitializerData.AIPatrolSystemDefinition);
            SightObjectSystem = new SightObjectSystem(this, this.AggressiveObjectInitializerData.SightObjectSystemDefinition, (InteractiveObjectTag => InteractiveObjectTag.IsPlayer),
                OnSightObjectSystemJustIntersected, OnSightObjectSystemIntersectedNothing, OnSightObjectSystemNoMoreIntersected);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, this.AggressiveObjectInitializerData.TransformMoveManagerComponentV3, this.OnAIDestinationReached,
                (unscaledSpeed) => this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectLocalDirection(unscaledSpeed));
            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, this.AggressiveObjectInitializerData.LocomotionAnimation);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            if (AIPatrollingState.isPatrolling) AIPatrolSystem.Tick(d);

            AIMoveToDestinationSystem.Tick(d);
        }

        public override void AfterTicks(float d)
        {
            base.AfterTicks(d);
            this.AIMoveToDestinationSystem.AfterTicks();
        }

        public void OnAIDestinationReached()
        {
            AIPatrolSystem.OnAIDestinationReached();
        }

        #region Sight Event

        protected void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            AIPatrollingState.isPatrolling = false;
            SetAISpeedAttenuationFactor(AIMovementSpeedDefinition.RUN);
            SetDestination(new ForwardAgentMovementCalculationStrategy(new AIDestination {WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition}));
        }

        protected void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }

        protected void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (SightObjectSystem.CurrentlyIntersectedInteractiveObjects.Count > 0)
                OnSightObjectSystemJustIntersected(SightObjectSystem.CurrentlyIntersectedInteractiveObjects[0]);
            else
                AIPatrollingState.isPatrolling = true;
        }

        #endregion


        public override void SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            AIMoveToDestinationSystem.SetDestination(IAgentMovementCalculationStrategy);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }
    }
}