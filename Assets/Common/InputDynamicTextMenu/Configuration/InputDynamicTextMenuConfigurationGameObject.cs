using UnityEngine;

namespace InputDynamicTextMenu
{
    public class InputDynamicTextMenuConfigurationGameObject : MonoBehaviour
    {
        private static InputDynamicTextMenuConfigurationGameObject Instance;

        public static InputDynamicTextMenuConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<InputDynamicTextMenuConfigurationGameObject>();
            }

            return Instance;
        }

        public InputDynamicTextMenuConfiguration InputDynamicTextMenuConfiguration;
    }
}