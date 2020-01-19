using CoreGame;
using UnityEngine;

namespace Skill
{
    public class SkillSlotUI
    {
        public const float VerticalLocalPositionInPercentageFromCenter = -0.38f;
        public const float SubSkillSpaceBetween = -0.05f;

        private SkillSlotUIGameObject SkillSlotUIGameObject;

        public SkillSlotUI(ref SKillSlotUIPositionInput SKillSlotUIPositionInput)
        {
            var instanciatedObject = MonoBehaviour.Instantiate(SkillSlotUIGlobalConfigurationGameObject.Get().SkillSlotUIGlobalConfiguration.SkillSlotUIBasePrefab, CoreGameSingletonInstances.GameCanvas.transform);
            this.SkillSlotUIGameObject = instanciatedObject.GetComponent<SkillSlotUIGameObject>();

            this.SkillSlotUIGameObject.Init();

            this.SkillSlotUIGameObject.PositionAndScaleSkillSlot(ref SKillSlotUIPositionInput);
        }

        public void SetCooldownProgress(float cooldownProgress)
        {
            this.SkillSlotUIGameObject.SetCooldownProgress(cooldownProgress);
        }

        public void Destroy()
        {
            GameObject.Destroy(this.SkillSlotUIGameObject.gameObject);
        }

        public void OnSkillActionChanged(SkillActionDefinition newSkillActionDefinition)
        {
            this.SkillSlotUIGameObject.UpdateSkillSlotUIFromSkillActionDefinition(newSkillActionDefinition);
        }

        public void SetInputText(string text)
        {
            this.SkillSlotUIGameObject.SetInputText(text);
        }
    }
}