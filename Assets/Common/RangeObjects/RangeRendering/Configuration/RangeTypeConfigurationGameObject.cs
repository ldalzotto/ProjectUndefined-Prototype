using UnityEngine;

namespace RangeObjects
{
    public class RangeTypeConfigurationGameObject : MonoBehaviour
    {
        private static RangeTypeConfigurationGameObject Instance;

        [SerializeField] private RangeRenderingConfiguration rangeRenderingConfiguration;
        public RangeRenderingConfiguration RangeRenderingConfiguration => rangeRenderingConfiguration;

        public static RangeTypeConfigurationGameObject Get()
        {
            if (Instance == null)
                Instance = FindObjectOfType<RangeTypeConfigurationGameObject>();
            return Instance;
        }
    }
}