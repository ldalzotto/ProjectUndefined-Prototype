using UnityEngine;

namespace Skill
{
    public class SkillSlotUIGlobalConfigurationGameObject : MonoBehaviour
    {
        public static SkillSlotUIGlobalConfigurationGameObject Instance;

        public static SkillSlotUIGlobalConfigurationGameObject Get()
        {
            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<SkillSlotUIGlobalConfigurationGameObject>();
            }

            return Instance;
        }

        public SkillSlotUIGlobalConfiguration SkillSlotUIGlobalConfiguration;
    }
}