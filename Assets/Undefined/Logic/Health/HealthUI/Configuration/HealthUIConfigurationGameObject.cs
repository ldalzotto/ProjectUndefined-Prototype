using UnityEngine;

namespace Health
{
    public class HealthUIConfigurationGameObject : MonoBehaviour
    {
        private static HealthUIConfigurationGameObject Instance;

        public static HealthUIConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<HealthUIConfigurationGameObject>();
            }

            return Instance;
        }

        public HealthUIConfiguration HealthUIConfiguration;
    }
}