using UnityEngine;

namespace Tutorial
{
    public class TutorialConfigurationGameObject : MonoBehaviour
    {
        private static TutorialConfigurationGameObject Instance;

        public static TutorialConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<TutorialConfigurationGameObject>();
            }

            return Instance;
        }

        public TutorialStepConfiguration TutorialStepConfiguration;
    }
}