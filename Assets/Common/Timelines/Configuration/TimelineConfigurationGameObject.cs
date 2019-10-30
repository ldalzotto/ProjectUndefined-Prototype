using UnityEngine;

namespace Timelines
{
    public class TimelineConfigurationGameObject : MonoBehaviour
    {
        private static TimelineConfigurationGameObject Instance;

        public static TimelineConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<TimelineConfigurationGameObject>();
            }

            return Instance;
        }

        public TimelineConfiguration TimelineConfiguration;
    }
}