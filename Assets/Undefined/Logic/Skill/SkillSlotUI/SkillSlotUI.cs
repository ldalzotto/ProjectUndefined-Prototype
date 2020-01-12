using CoreGame;
using UnityEngine;

namespace Skill
{
    public class SkillSlotUI
    {
        public const float VerticalLocalPositionInPercentageFromCenter = -0.38f;

        private SkillSlotUIGameObject SkillSlotUIGameObject;

        public SkillSlotUI(SKillSlotUIPositionInput SKillSlotUIPositionInput)
        {
            var instanciatedObject = MonoBehaviour.Instantiate(SkillSlotUIGlobalConfigurationGameObject.Get().SkillSlotUIGlobalConfiguration.SkillSlotUIBasePrefab, CoreGameSingletonInstances.GameCanvas.transform);
            this.SkillSlotUIGameObject = instanciatedObject.GetComponent<SkillSlotUIGameObject>();
            this.SkillSlotUIGameObject.Init();

            this.SkillSlotUIGameObject.PositionAndScaleSkillSlot(SKillSlotUIPositionInput);

            /// Main weapon
            /*
            this.SkillSlotUIGameObject.Reset(RectTransformSetup.BOTTOM_RIGHT);
            this.SkillSlotUIGameObject.sizeDelta = new Vector2(100, 100);
            this.SkillSlotUIGameObject.SetPivot(RectTransformSetup.BOTTOM_RIGHT);
            this.SkillSlotUIGameObject.SetLocalPositionRelativeToCanvasScaler(new Vector2(0.48f, VerticalLocalPositionInPercentageFromCenter));

            (this._skillSlotUiGameObject.transform as RectTransform).sizeDelta = new Vector2(100, 53.9f);
            (this._skillSlotUiGameObject.SkillSlotUIconIGameObject.transform as RectTransform).sizeDelta = new Vector2(70f, 70f);
            */

            /// Sub skill
            /*
            this.SkillSlotUIGameObject.Reset(RectTransformSetup.BOTTOM_LEFT);
            this.SkillSlotUIGameObject.sizeDelta = new Vector2(100, 100);
            this.SkillSlotUIGameObject.SetPivot(RectTransformSetup.BOTTOM_LEFT);
            this.SkillSlotUIGameObject.SetLocalPositionRelativeToCanvasScaler(new Vector2(-0.48f, VerticalLocalPositionInPercentageFromCenter));

            (this._skillSlotUiGameObject.transform as RectTransform).sizeDelta = new Vector2(40f, 40f);
            (this._skillSlotUiGameObject.SkillSlotUIconIGameObject.transform as RectTransform).sizeDelta = new Vector2(35f, 35f);
            */
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
            if (newSkillActionDefinition != null)
            {
                this.SkillSlotUIGameObject.SetUIIcon(newSkillActionDefinition.SkillIcon);
            }
        }
    }
}