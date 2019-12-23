using CoreGame;
using InteractiveObjects;
using UnityEngine;

namespace HealthGlobe
{
    /// <summary>
    /// This class is desgined to only hold a <see cref="HealthGlobeInteractiveObjectDefinition"/> references.
    /// This is used for prefab instanciation where <see cref="HealthGlobeInteractiveObjectDefinition"/> is used as a template.
    /// /!\ This component is destroyed after instanciation.
    /// </summary>
    [SceneHandleDraw]
    public class HealthGlobeInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition;

        private HealthGlobeInteractiveObjectDefinitionStruct RuntimeHealthGlobeInteractiveObjectDefinition;
        private BeziersControlPointsBuildInput BeziersControlPointsBuildInput;

        public void SetupBeforeObjectCreation(float RecoveredHealth, BeziersControlPointsBuildInput BeziersControlPointsBuildInput)
        {
            this.RuntimeHealthGlobeInteractiveObjectDefinition = this.HealthGlobeInteractiveObjectDefinition;
            this.RuntimeHealthGlobeInteractiveObjectDefinition.SetRecoveredHealthValue(RecoveredHealth);
            this.BeziersControlPointsBuildInput = BeziersControlPointsBuildInput;
        }

        protected override CoreInteractiveObject InitializationLogic()
        {
            return new HealthGlobeInteractiveObject(this.RuntimeHealthGlobeInteractiveObjectDefinition, InteractiveGameObjectFactory.Build(this.gameObject), this.BeziersControlPointsBuildInput);
        }
    }
}