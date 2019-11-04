using System;
using OdinSerializer;

namespace Health
{
    [Serializable]
    public class HealthSystemDefinition : SerializedScriptableObject
    {
        public float StartHealth;
    }
}