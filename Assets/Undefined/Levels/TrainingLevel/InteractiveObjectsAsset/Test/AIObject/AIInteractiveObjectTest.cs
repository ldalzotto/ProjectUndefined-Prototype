using AIObjects;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using VisualFeedback;

namespace InteractiveObjects
{
    public class AIInteractiveObjectTest : CoreInteractiveObject
    {
        #region State

        [VE_Nested] private AIPatrollingState AIPatrollingState;
        [VE_Nested] private AIAttractiveObjectState AIAttractiveObjectState;
        [VE_Nested] private AIDisarmObjectState AIDisarmObjectState;

        #endregion

        private AIInteractiveObjectTestInitializerData AIInteractiveObjectInitializerData;

        private AIPatrolSystem AIPatrolSystem;
        private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private LocalCutscenePlayerSystem LocalCutscenePlayerSystem;
        [VE_Nested] private SightObjectSystem SightObjectSystem;
        private LineVisualFeedbackSystem LineVisualFeedbackSystem;

        public AIInteractiveObjectTest(IInteractiveGameObject interactiveGameObject, AIInteractiveObjectTestInitializerData AIInteractiveObjectInitializerData)
        {
            this.AIInteractiveObjectInitializerData = AIInteractiveObjectInitializerData;
            interactiveGameObject.CreateLogicCollider(AIInteractiveObjectInitializerData.InteractiveObjectLogicCollider);
            interactiveGameObject.CreateAgent(AIInteractiveObjectInitializerData.AIAgentDefinition);
            base.BaseInit(interactiveGameObject);
        }

        public override void Init()
        {
            this.AIPatrollingState = new AIPatrollingState();
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, AIInteractiveObjectInitializerData.TransformMoveManagerComponentV3, this.OnAIDestinationReached);
            this.AIAttractiveObjectState = new AIAttractiveObjectState(new BoolVariable(false, OnAIIsJustAttractedByAttractiveObject, OnAIIsNoMoreAttractedByAttractiveObject));
            this.AIDisarmObjectState = new AIDisarmObjectState(new BoolVariable(false, OnAIIsJustDisarmingObject, OnAIIsNoMoreJustDisarmingObject));
            this.interactiveObjectTag = new InteractiveObjectTag {IsAi = true};

            this.AIPatrolSystem = new AIPatrolSystem(this, AIInteractiveObjectInitializerData.AIPatrolSystemDefinition);

            this.SightObjectSystem = new SightObjectSystem(this, AIInteractiveObjectInitializerData.SightObjectSystemDefinition,
                (InteractiveObjectTag => InteractiveObjectTag.IsAttractiveObject),
                OnSightObjectSystemJustIntersected, OnSightObjectSystemIntersectedNothing, OnSightObjectSystemNoMoreIntersected);
            this.LocalCutscenePlayerSystem = new LocalCutscenePlayerSystem();
            this.LineVisualFeedbackSystem = new LineVisualFeedbackSystem(this.InteractiveGameObject);
        }

        public override void Tick(float d)
        {
            LocalCutscenePlayerSystem.Tick(d);
            if (!AIDisarmObjectState.IsDisarming.GetValue() && !AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue()) AIPatrollingState.isPatrolling = true;

            if (AIPatrollingState.isPatrolling) AIPatrolSystem.Tick(d);

            AIMoveToDestinationSystem.Tick(d);
            LineVisualFeedbackSystem.Tick(d);
            base.Tick(d);
        }

        public override void AfterTicks(float d)
        {
            this.AIPatrolSystem.AfterTicks();
            this.AIMoveToDestinationSystem.AfterTicks();
            base.AfterTicks(d);
        }

        public void OnAIDestinationReached()
        {
            AIPatrolSystem.OnAIDestinationReached();
        }

        public override void SetAIDestination(AIDestination AIDestination)
        {
            this.AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            this.AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }

        public override void OnOtherDisarmObjectTriggerEnter(CoreInteractiveObject OtherInteractiveObject)
        {
            AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            AIMoveToDestinationSystem.ClearPath();
            AIDisarmObjectState.IsDisarming.SetValue(true);
        }

        public override void OnOtherDisarmobjectTriggerExit(CoreInteractiveObject OtherInteractiveObject)
        {
            AIDisarmObjectState.IsDisarming.SetValue(false);
        }

        public void OnAIIsJustDisarmingObject()
        {
            LocalCutscenePlayerSystem.PlayCutscene(AIInteractiveObjectInitializerData.DisarmObjectAnimation.GetSequencedActions(this));
        }

        public void OnAIIsNoMoreJustDisarmingObject()
        {
            LocalCutscenePlayerSystem.KillCurrentCutscene();
        }

        private void OnSightObjectSystemJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            if (!AIDisarmObjectState.IsDisarming.GetValue())
            {
                AIPatrollingState.isPatrolling = false;
                SwitchToAttractedState(IntersectedInteractiveObject);
                if (!AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue()) LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, IntersectedInteractiveObject);
            }
        }

        private void OnSightObjectSystemIntersectedNothing(CoreInteractiveObject IntersectedInteractiveObject)
        {
            OnSightObjectSystemJustIntersected(IntersectedInteractiveObject);
        }

        private void OnSightObjectSystemNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
        }

        #region Attractive Object

        public override void OnOtherAttractiveObjectJustIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!AIDisarmObjectState.IsDisarming.GetValue()) SwitchToAttractedState(OtherInteractiveObject);
        }

        private void SwitchToAttractedState(CoreInteractiveObject OtherInteractiveObject)
        {
            if (OtherInteractiveObject.InteractiveObjectTag.IsAttractiveObject || OtherInteractiveObject.InteractiveObjectTag.IsPlayer)
            {
                AIAttractiveObjectState.SetIsAttractedByAttractiveObject(true, OtherInteractiveObject);
                AIPatrollingState.isPatrolling = false;
                SetAIDestination(new AIDestination {WorldPosition = OtherInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition});
            }
        }

        public override void OnOtherAttractiveObjectIntersectedNothing(CoreInteractiveObject OtherInteractiveObject)
        {
            if (!AIDisarmObjectState.IsDisarming.GetValue() && !AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue()) SwitchToAttractedState(OtherInteractiveObject);
        }

        public override void OnOtherAttractiveObjectNoMoreIntersected(CoreInteractiveObject OtherInteractiveObject)
        {
            if (AIAttractiveObjectState.IsAttractedByAttractiveObject.GetValue())
            {
                AIMoveToDestinationSystem.ClearPath();
                AIAttractiveObjectState.SetIsAttractedByAttractiveObject(false, OtherInteractiveObject);
            }

            LineVisualFeedbackSystem.DestroyLine(OtherInteractiveObject);
        }

        public void OnAIIsJustAttractedByAttractiveObject()
        {
            SetAISpeedAttenuationFactor(AIInteractiveObjectInitializerData.AISpeedWhenAttracted);
            LineVisualFeedbackSystem.CreateLineFollowing(DottedLineID.ATTRACTIVE_OBJECT, AIAttractiveObjectState.AttractedInteractiveObject);
        }

        public void OnAIIsNoMoreAttractedByAttractiveObject()
        {
            LineVisualFeedbackSystem.DestroyLine(AIAttractiveObjectState.AttractedInteractiveObject);
        }

        #endregion
    }
}