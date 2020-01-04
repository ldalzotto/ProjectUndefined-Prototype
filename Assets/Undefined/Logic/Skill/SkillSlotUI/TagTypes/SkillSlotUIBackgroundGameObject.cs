using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    public class SkillSlotUIBackgroundGameObject : MonoBehaviour
    {
        public Material SkillIconBackgroundMaterialInstance;

        public void Init()
        {
            this.SkillIconBackgroundMaterialInstance = GetComponent<Image>().material;
        }

        public void SetCooldownProgress(float cooldownProgress)
        {
            this.SkillIconBackgroundMaterialInstance.SetFloat("_CooldownProgress", cooldownProgress);
        }
    }
}