using UnityEngine;

namespace Tests
{
    public class TestControllerConfiguration : MonoBehaviour
    {

        private static TestControllerConfiguration Instance;

        public static TestControllerConfiguration Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<TestControllerConfiguration>();
            }

            return Instance;
        }

        [Inline()]
        public TestControllerDefinition TestControllerDefinition;
    }
}