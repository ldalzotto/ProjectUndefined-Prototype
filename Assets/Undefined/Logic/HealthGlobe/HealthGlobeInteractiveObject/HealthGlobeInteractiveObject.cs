using CoreGame;
using Health;
using InteractiveObjects;
using InteractiveObjects_Interfaces;

namespace HealthGlobe
{
    public class HealthGlobeInteractiveObject : CoreInteractiveObject
    {
        private HealthGlobeInteractiveObjectDefinitionStruct HealthGlobeInteractiveObjectDefinitionStruct;

        #region Systems

        private BeziersMovementSystem SpawnBeziersMovementSystem;

        /// <summary>
        /// /!\ The <see cref="RecoveringHealthEmitterSystem"/> is disabled until the <see cref="SpawnBeziersMovementSystem"/> has ended.
        /// For every operation, we must check <see cref="RecoveringHealthEmitterSystem"/> nullity.
        /// </summary>
        private RecoveringHealthEmitterSystem RecoveringHealthEmitterSystem;

        #endregion

        public HealthGlobeInteractiveObject(HealthGlobeInteractiveObjectDefinitionStruct healthGlobeInteractiveObjectDefinitionStruct,
            IInteractiveGameObject interactiveGameObject, BeziersControlPointsBuildInput BeziersControlPointsBuildInput, bool IsUpdatedInMainManager = true)
        {
            this.HealthGlobeInteractiveObjectDefinitionStruct = healthGlobeInteractiveObjectDefinitionStruct;
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
            this.RecoveringHealthEmitterSystem = new RecoveringHealthEmitterSystem(this, HealthGlobeInteractiveObjectDefinitionStruct.RecoveringHealthEmitterSystemDefinitionStruct,
                OnHealthRecoveredCallback: this.OnHealthRecovered);
        }

        private void OnHealthRecovered()
        {
            this.isAskingToBeDestroyed = true;
        }
    }
}