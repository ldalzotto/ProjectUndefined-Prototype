using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace HealthGlobe
{
    public class HealthGlobeInteractiveObject : CoreInteractiveObject
    {
        private RecoveringHealthEmitterSystem RecoveringHealthEmitterSystem;

        public HealthGlobeInteractiveObject(HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition,
            IInteractiveGameObject interactiveGameObject, bool IsUpdatedInMainManager = true)
        {
            base.BaseInit(interactiveGameObject, IsUpdatedInMainManager);
            this.RecoveringHealthEmitterSystem = new RecoveringHealthEmitterSystem(this, HealthGlobeInteractiveObjectDefinition.RecoveringHealthEmitterSystemDefinition,
                OnHealthRecoveredCallback: this.OnHealthRecovered);
        }

        public override void Init()
        {
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.RecoveringHealthEmitterSystem.Tick(d);
        }

        public override void Destroy()
        {
            this.RecoveringHealthEmitterSystem.Destroy();
            base.Destroy();
        }

        private void OnHealthRecovered()
        {
            this.isAskingToBeDestroyed = true;
        }
    }
}