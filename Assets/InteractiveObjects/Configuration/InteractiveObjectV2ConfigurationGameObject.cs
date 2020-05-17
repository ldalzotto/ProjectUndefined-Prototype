using UnityEngine;

namespace InteractiveObjects
{
    public class InteractiveObjectV2ConfigurationGameObject : MonoBehaviour
    {
        private static InteractiveObjectV2ConfigurationGameObject Instance;
        public static InteractiveObjectV2ConfigurationGameObject Get()
        {
            if (Instance == null) { Instance = GameObject.FindObjectOfType<InteractiveObjectV2ConfigurationGameObject>(); }
            return Instance;
        }

        [SerializeField]
        private InteractiveObjectV2Configuration interactiveObjectV2Configuration;
        public InteractiveObjectV2Configuration InteractiveObjectV2Configuration { get => interactiveObjectV2Configuration; }
    }
}
