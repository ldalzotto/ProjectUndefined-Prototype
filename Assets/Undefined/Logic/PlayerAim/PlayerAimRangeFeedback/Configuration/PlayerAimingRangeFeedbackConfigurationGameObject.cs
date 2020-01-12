using UnityEngine;
using UnityEngine.Serialization;

namespace FiredProjectile
{
    public class PlayerAimingRangeFeedbackConfigurationGameObject : MonoBehaviour
    {
        private static PlayerAimingRangeFeedbackConfigurationGameObject Instance;

        public static PlayerAimingRangeFeedbackConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<PlayerAimingRangeFeedbackConfigurationGameObject>();
            }

            return Instance;
        }

        [FormerlySerializedAs("firingRangeVisualFeedbackConfiguration")] [FormerlySerializedAs("playerFiringRangeVisualFeedbackConfiguration")]
        public PlayerAimingRangeVisualFeedbackConfiguration playerAimingRangeVisualFeedbackConfiguration;
    }
}