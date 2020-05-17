using UnityEngine;

namespace Persistence
{
    public class PersistanceConfigurationGameObject : MonoBehaviour
    {
        private static PersistanceConfigurationGameObject Instance;

        public static PersistanceConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<PersistanceConfigurationGameObject>();
            }

            return Instance;
        }

        public PersistanceConfiguration PersistanceConfiguration;
    }
}