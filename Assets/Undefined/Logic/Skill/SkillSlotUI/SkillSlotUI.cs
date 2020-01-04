using CoreGame;
using UnityEngine;

namespace Skill
{
    public class SkillSlotUI
    {
        private RectTransform SkillSlotUIGameObject;

        private SkillSlotUIBackgroundGameObject SkillSlotUIBackgroundGameObject;

        public SkillSlotUI()
        {
            this.SkillSlotUIGameObject = MonoBehaviour.Instantiate(SkillSlotUIGlobalConfigurationGameObject.Get().SkillSlotUIGlobalConfiguration.SkillSlotUIBasePrefab, CoreGameSingletonInstances.GameCanvas.transform);
            this.SkillSlotUIBackgroundGameObject = this.SkillSlotUIGameObject.GetComponentInChildren<SkillSlotUIBackgroundGameObject>();
            this.SkillSlotUIBackgroundGameObject.Init();
        }

        public void SetCooldownProgress(float cooldownProgress)
        {
            this.SkillSlotUIBackgroundGameObject.SetCooldownProgress(cooldownProgress);
        }

        public void Destroy()
        {
            GameObject.Destroy(this.SkillSlotUIGameObject.gameObject);
        }
    }
}