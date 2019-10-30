using UnityEngine;

namespace VisualFeedback
{
    public class CircleFillBarConfigurationGameObject : MonoBehaviour
    {
        private static CircleFillBarConfigurationGameObject Instance;

        public static CircleFillBarConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<CircleFillBarConfigurationGameObject>();
            }

            return Instance;
        }

        public CircleFillBarConfiguration CircleFillBarConfiguration;
    }
}