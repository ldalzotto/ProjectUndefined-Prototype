using UnityEngine;

namespace LevelManagement
{
    public class LevelConfigurationGameObject : MonoBehaviour
    {
        private static LevelConfigurationGameObject Instance;

        public static LevelConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<LevelConfigurationGameObject>();
            }

            return Instance;
        }

        public LevelConfigurationData LevelConfigurationData;
    }
}