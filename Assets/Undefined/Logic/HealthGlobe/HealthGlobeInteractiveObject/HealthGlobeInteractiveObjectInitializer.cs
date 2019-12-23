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
        [DrawNested]
        public HealthGlobeInteractiveObjectDefinition HealthGlobeInteractiveObjectDefinition;
    }
}