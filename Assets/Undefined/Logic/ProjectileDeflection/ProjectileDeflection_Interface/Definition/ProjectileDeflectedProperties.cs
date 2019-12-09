using System;
using OdinSerializer;

namespace ProjectileDeflection_Interface
{
    [Serializable]
    public class ProjectileDeflectedProperties : SerializedScriptableObject
    {
        public float HealthRecovered;

        public static implicit operator ProjectileDeflectedPropertiesStruct(ProjectileDeflectedProperties ProjectileDeflectedProperties)
        {
            return new ProjectileDeflectedPropertiesStruct(ProjectileDeflectedProperties.HealthRecovered);
        }
    }

    public struct ProjectileDeflectedPropertiesStruct
    {
        public float HealthRecovered;

        public ProjectileDeflectedPropertiesStruct(float healthRecovered)
        {
            HealthRecovered = healthRecovered;
        }
    }
}