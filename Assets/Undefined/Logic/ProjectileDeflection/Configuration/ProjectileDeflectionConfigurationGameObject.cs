using UnityEngine;

namespace ProjectileDeflection
{
    public class ProjectileDeflectionConfigurationGameObject : MonoBehaviour
    {
        private static ProjectileDeflectionConfigurationGameObject Instance;

        public static ProjectileDeflectionConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<ProjectileDeflectionConfigurationGameObject>();
            }

            return Instance;
        }

        public ProjectileDeflectionGlobalConfiguration ProjectileDeflectionGlobalConfiguration;
    }
}