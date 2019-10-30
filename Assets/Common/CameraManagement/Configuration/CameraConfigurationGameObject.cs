using UnityEngine;

namespace CameraManagement
{
    public class CameraConfigurationGameObject : MonoBehaviour
    {
        private static CameraConfigurationGameObject Instance;

        public static CameraConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<CameraConfigurationGameObject>();
            }

            return Instance;
        }

        public CameraMovementConfiguration CameraMovementConfiguration;
    }
}