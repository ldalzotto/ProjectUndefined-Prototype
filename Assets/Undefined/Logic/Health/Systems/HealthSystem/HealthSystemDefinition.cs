using System;
using OdinSerializer;
using ParticleObjects;

namespace Health
{
    [Serializable]
    public class HealthSystemDefinition : SerializedScriptableObject
    {
        public float StartHealth;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public ParticleObjectDefinition HealthRecoveryParticleEffect;
    }
}