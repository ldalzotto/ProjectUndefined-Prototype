using System;
using OdinSerializer;

namespace HealthGlobe
{
    [Serializable]
    [SceneHandleDraw]
    public class HealthGlobeSpawnDefinition : SerializedScriptableObject
    {
        public float RecoveredHealth;
        [WireCircle()] public float MinDistanceFromSource;
        [WireCircle()] public float MaxDistanceFromSource;
        public int MinNumberOfGlobes;
        public int MaxNumberOfGlobes;
    }
}