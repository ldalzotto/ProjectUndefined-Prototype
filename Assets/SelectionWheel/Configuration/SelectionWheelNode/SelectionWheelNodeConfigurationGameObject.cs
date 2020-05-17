using UnityEngine;

namespace SelectionWheel
{
    public class SelectionWheelNodeConfigurationGameObject : MonoBehaviour
    {
        private static SelectionWheelNodeConfigurationGameObject Instance;

        [SerializeField] private SelectionWheelNodeConfiguration selectionWheelNodeConfiguration;

        public SelectionWheelNodeConfiguration SelectionWheelNodeConfiguration => selectionWheelNodeConfiguration;

        public static SelectionWheelNodeConfigurationGameObject Get()
        {
            if (Instance == null) Instance = FindObjectOfType<SelectionWheelNodeConfigurationGameObject>();

            return Instance;
        }
    }
}