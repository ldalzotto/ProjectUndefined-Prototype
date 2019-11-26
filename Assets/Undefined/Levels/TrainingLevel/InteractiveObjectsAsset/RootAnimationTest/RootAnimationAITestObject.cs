using AIObjects;
using InteractiveObject_Animation;
using InteractiveObjects_Interfaces;
using UnityEngine.AI;

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
                (unscaledSpeed) => this.BaseObjectAnimatorPlayableSystem.SetUnscaledObjectLocalDirection(unscaledSpeed));
            this.BaseObjectAnimatorPlayableSystem = new BaseObjectAnimatorPlayableSystem(this.AnimationController, this.RootAnimationAITestObjectInitializerData.LocomotionAnimation);
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

        public override NavMeshPathStatus SetDestination(IAgentMovementCalculationStrategy IAgentMovementCalculationStrategy)
        {
            return AIMoveToDestinationSystem.SetDestination(IAgentMovementCalculationStrategy);
        }

        public override void SetAISpeedAttenuationFactor(AIMovementSpeedAttenuationFactor aiMovementSpeedAttenuationFactor)
        {
            AIMoveToDestinationSystem.SetSpeedAttenuationFactor(aiMovementSpeedAttenuationFactor);
        }
    }
}