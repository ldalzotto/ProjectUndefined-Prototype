using UnityEngine;

namespace VisualFeedback
{
    public class VisualFeedbackConfigurationGameObject : MonoBehaviour
    {
        private static VisualFeedbackConfigurationGameObject Instance;

        public static VisualFeedbackConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<VisualFeedbackConfigurationGameObject>();
            }

            return Instance;
        }

        public DottedLineConfiguration DottedLineConfiguration;
        public DottedLineStaticConfiguration DottedLineStaticConfiguration;
    }
}