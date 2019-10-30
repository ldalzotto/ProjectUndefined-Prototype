using UnityEngine;

namespace LevelManagement
{
    public class LevelManagementConfigurationGameObject : MonoBehaviour
    {
        private static LevelManagementConfigurationGameObject Instance;

        public static LevelManagementConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<LevelManagementConfigurationGameObject>();
            }

            return Instance;
        }

        public ChunkZonesSceneConfiguration ChunkZonesSceneConfiguration;
        public LevelHierarchyConfiguration LevelHierarchyConfiguration;
        public LevelZonesSceneConfiguration LevelZonesSceneConfiguration;
        public GlobalLevelConfiguration GlobalLevelConfiguration;
    }
}