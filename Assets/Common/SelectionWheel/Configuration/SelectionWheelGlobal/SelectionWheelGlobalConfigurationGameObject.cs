using UnityEngine;

namespace SelectionWheel
{
    public class SelectionWheelGlobalConfigurationGameObject : MonoBehaviour
    {
        private static SelectionWheelGlobalConfigurationGameObject Instace;

        [SerializeField] private SelectionWheelGlobalConfiguration selectionWheelGlobalConfiguration;
        public SelectionWheelGlobalConfiguration SelectionWheelGlobalConfiguration => selectionWheelGlobalConfiguration;

        public static SelectionWheelGlobalConfigurationGameObject Get()
        {
            if (Instace == null) Instace = FindObjectOfType<SelectionWheelGlobalConfigurationGameObject>();

            return Instace;
        }
    }
}