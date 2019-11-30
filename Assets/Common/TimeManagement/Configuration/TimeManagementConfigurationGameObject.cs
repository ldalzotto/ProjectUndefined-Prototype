using UnityEngine;

namespace TimeManagement
{
    public class TimeManagementConfigurationGameObject : MonoBehaviour
    {
        private static TimeManagementConfigurationGameObject Instance;

        public static TimeManagementConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<TimeManagementConfigurationGameObject>();
            }

            return Instance;
        }

        public TimeManagementConfiguration TimeManagementConfiguration;
    }
}