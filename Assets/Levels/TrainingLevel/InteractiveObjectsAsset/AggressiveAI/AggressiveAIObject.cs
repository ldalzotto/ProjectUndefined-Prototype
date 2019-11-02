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
            interactiveObjectTag = new InteractiveObjectTag {IsAi = 1};
            AIPatrollingState = new AIPatrollingState();
            AIPatrollingState.isPatrolling = true;
            AIPatrolSystem = new AIPatrolSystem(this, this.AggressiveObjectInitializerData.AIPatrolSystemDefinition);
            SightObjectSystem = new SightObjectSystem(this, this.AggressiveObjectInitializerData.SightObjectSystemDefinition, new InteractiveObjectTag {IsPlayer = 1},
                OnSightObjectSystemJustIntersected, OnSightObjectSystemIntersectedNothing, OnSightObjectSystemNoMoreIntersected);
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, this.AggressiveObjectInitializerData, this.OnAIDestinationReached);
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

        public override void OnAIDestinationReached()
        {
            AIPatrolSystem.OnAIDestinationReached();
        }

        #region Sight Event

        protected void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            AIPatrollingState.isPatrolling = false;
            SetAISpeedAttenuationFactor(AIMovementSpeedDefinition.RUN);
            SetAIDestination(new AIDestination {WorldPosition = IntersectedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition});
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


        public override void SetAIDestination(AIDestination AIDestination)
        {
            AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        public override void OnAnimationObjectSetUnscaledSpeedMagnitude(float unscaledSpeedMagnitude)
        {
            this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectSpeed(unscaledSpeedMagnitude);
        }
    }
}