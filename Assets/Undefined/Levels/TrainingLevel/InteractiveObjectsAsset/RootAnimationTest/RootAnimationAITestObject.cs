using AIObjects;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    public class RootAnimationAITestObject : CoreInteractiveObject
    {
        private RootAnimationAITestObjectInitializerData RootAnimationAITestObjectInitializerData;

        private LocalCutscenePlayerSystem LocalCutscenePlayerSystem;
        [VE_Nested] private AIMoveToDestinationSystem AIMoveToDestinationSystem;
        private BaseObjectAnimatorPlayableSystem BaseObjectAnimatorPlayableSystem;

        public RootAnimationAITestObject(IInteractiveGameObject interactiveGameObject, RootAnimationAITestObjectInitializerData RootAnimationAITestObjectInitializerData)
        {
            this.RootAnimationAITestObjectInitializerData = RootAnimationAITestObjectInitializerData;
            interactiveGameObject.CreateAgent(this.RootAnimationAITestObjectInitializerData.AIAgentDefinition);
            interactiveGameObject.CreateLogicCollider(this.RootAnimationAITestObjectInitializerData.InteractiveObjectLogicCollider);
            base.BaseInit(interactiveGameObject);
        }

        public override void Init()
        {
            this.interactiveObjectTag = new InteractiveObjectTag() {IsAi = true};
            this.LocalCutscenePlayerSystem = new LocalCutscenePlayerSystem();
            this.AIMoveToDestinationSystem = new AIMoveToDestinationSystem(this, this.RootAnimationAITestObjectInitializerData.TransformMoveManagerComponentV3, OnAIDestinationReached,
                (unscaledSpeed) => this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectSpeed(unscaledSpeed));
            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimatorPlayable, this.RootAnimationAITestObjectInitializerData.LocomotionAnimation);
            this.LocalCutscenePlayerSystem.PlayCutscene(this.RootAnimationAITestObjectInitializerData.RootAnimationCutsceneTemplate.GetSequencedActions(this));
        }

        public override void Tick(float d)
        {
            this.AIMoveToDestinationSystem.Tick(d);
            this.LocalCutscenePlayerSystem.Tick(d);
            this.BaseObjectAnimatorPlayableSystem.Tick(d);
            base.Tick(d);
        }

        public override void AfterTicks(float d)
        {
            this.AIMoveToDestinationSystem.AfterTicks();
        }

        protected override void OnRootMotionEnabled()
        {
            this.AIMoveToDestinationSystem.IsEnabled = false;
        }

        protected override void OnRootMotionDisabled()
        {
            this.AIMoveToDestinationSystem.IsEnabled = true;
        }

        public void OnAIDestinationReached()
        {
            this.LocalCutscenePlayerSystem.OnAIDestinationReached();
        }

        public override void SetAIDestination(AIDestination AIDestination)
        {
            AIMoveToDestinationSystem.SetDestination(AIDestination);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            AIMoveToDestinationSystem.SetSpeedAttenuationFactor(AIMovementSpeedDefinition);
        }
    }
}