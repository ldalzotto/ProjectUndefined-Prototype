using CoreGame;
using UnityEngine;

namespace PlayerDash
{
    public class PlayerDashConfigurationGameObject : MonoBehaviour
    {
        private static PlayerDashConfigurationGameObject Instance;

        public static PlayerDashConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<PlayerDashConfigurationGameObject>();
            }

            return Instance;
        }

        public PlayerDashConfiguration PlayerDashConfiguration;
    }
}