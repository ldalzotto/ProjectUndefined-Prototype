using UnityEngine;

namespace SelectableObject
{
    public class SelectableObjectsConfigurationGameObject : MonoBehaviour
    {
        private static SelectableObjectsConfigurationGameObject Instance;

        public static SelectableObjectsConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<SelectableObjectsConfigurationGameObject>();
            }

            return Instance;
        }

        public SelectableObjectsConfiguration SelectableObjectsConfiguration;
    }
}