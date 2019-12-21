using UnityEngine;

namespace HealthGlobe
{
    public class GlobalHealthGlobeConfigurationGameObject : MonoBehaviour
    {
        private static GlobalHealthGlobeConfigurationGameObject Instance;

        public static GlobalHealthGlobeConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<GlobalHealthGlobeConfigurationGameObject>();
            }

            return Instance;
        }

        public GlobalHealthGlobeConfiguration GlobalHealthGlobeConfiguration;
    }
}