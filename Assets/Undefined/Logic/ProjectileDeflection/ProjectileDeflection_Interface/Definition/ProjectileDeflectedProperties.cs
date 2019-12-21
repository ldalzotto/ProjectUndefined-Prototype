using System;
using HealthGlobe;
using OdinSerializer;

namespace ProjectileDeflection_Interface
{
    [Serializable]
    [SceneHandleDraw]
    public class ProjectileDeflectedProperties : SerializedScriptableObject
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        [DrawNested]
        public HealthGlobeSpawnDefinition HealthGlobeRecoveryDefinition;
    }
}