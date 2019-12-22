using CoreGame;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace HealthGlobe
{
    public class HealthGlobeInteractiveObject : CoreInteractiveObject
    {
        private HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition;

        #region Systems

        private BeziersMovementSystem SpawnBeziersMovementSystem;
        
        /// <summary>
        /// /!\ The <see cref="RecoveringHealthEmitterSystem"/> is disabled until the <see cref="SpawnBeziersMovementSystem"/> has ended.
        /// For every operation, we must check <see cref="RecoveringHealthEmitterSystem"/> nullity.
        /// </summary>
        private RecoveringHealthEmitterSystem RecoveringHealthEmitterSystem;

        #endregion

        public HealthGlobeInteractiveObject(HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition,
            IInteractiveGameObject interactiveGameObject, BeziersControlPointsBuildInput BeziersControlPointsBuildInput, bool IsUpdatedInMainManager = true)
        {
            this.HealthGlobeInteractiveObjectDefinition = HealthGlobeInteractiveObjectDefinition;
            base.BaseInit(interactiveGameObject, IsUpdatedInMainManager);

            this.SpawnBeziersMovementSystem = new BeziersMovementSystem(BeziersControlPointsBuildInput, this.OnHealthGlobeSpawnBeziersMovementEnded);
            this.SpawnBeziersMovementSystem.StartBeziersMovement();
        }

        public override void Init()
        {
        }

        public override void Tick(float d)
        {
            this.SpawnBeziersMovementSystem.Tick(d);
            this.InteractiveGameObject.InteractiveGameObjectParent.transform.position = this.SpawnBeziersMovementSystem.GetBeziersPathPosition();
            this.RecoveringHealthEmitterSystem?.Tick(d);
        }

        public override void Destroy()
        {
            this.SpawnBeziersMovementSystem.Destroy();
            this.RecoveringHealthEmitterSystem?.Destroy();
            base.Destroy();
        }

        /// <summary>
        /// This event is called from <see cref="SpawnBeziersMovementSystem"/> and creates the <see cref="RecoveringHealthEmitterSystem"/> that allow player to recover
        /// health.
        /// </summary>
        private void OnHealthGlobeSpawnBeziersMovementEnded(BeziersMovementSystem BeziersMovementSystem)
        {
            this.RecoveringHealthEmitterSystem = new RecoveringHealthEmitterSystem(this, HealthGlobeInteractiveObjectDefinition.RecoveringHealthEmitterSystemDefinition,
                OnHealthRecoveredCallback: this.OnHealthRecovered);
        }

        private void OnHealthRecovered()
        {
            this.isAskingToBeDestroyed = true;
        }
    }
}