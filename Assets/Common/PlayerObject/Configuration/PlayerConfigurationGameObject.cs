using UnityEngine;

namespace PlayerObject
{
    public class PlayerConfigurationGameObject : MonoBehaviour
    {
        private static PlayerConfigurationGameObject Instance;

        public static PlayerConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<PlayerConfigurationGameObject>();
            }

            return Instance;
        }

        public PlayerGlobalConfiguration PlayerGlobalConfiguration;
    }
}