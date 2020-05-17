using UnityEngine;

namespace Input
{
    public class InputConfigurationGameObject : MonoBehaviour
    {
        private static InputConfigurationGameObject Instance;

        public static InputConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<InputConfigurationGameObject>();
            }

            return Instance;
        }

        public CoreInputConfiguration CoreInputConfiguration;
        public InputConfiguration InputConfiguration;
    }
}