using UnityEngine;

namespace Targetting
{
    public class TargettingConfigurationGameObject : MonoBehaviour
    {
        private static TargettingConfigurationGameObject Instance;

        public static TargettingConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<TargettingConfigurationGameObject>();
            }

            return Instance;
        }

        public TargettingConfiguration TargettingConfiguration;
    }
}