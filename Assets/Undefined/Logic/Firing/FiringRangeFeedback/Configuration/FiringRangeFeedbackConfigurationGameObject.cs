using UnityEngine;
using UnityEngine.Serialization;

namespace FiredProjectile
{
    public class FiringRangeFeedbackConfigurationGameObject : MonoBehaviour
    {
        private static FiringRangeFeedbackConfigurationGameObject Instance;

        public static FiringRangeFeedbackConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<FiringRangeFeedbackConfigurationGameObject>();
            }

            return Instance;
        }

       [FormerlySerializedAs("playerFiringRangeVisualFeedbackConfiguration")] public FiringRangeVisualFeedbackConfiguration firingRangeVisualFeedbackConfiguration;
    }
}