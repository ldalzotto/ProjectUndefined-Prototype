using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace Skill
{
    public class SkillSlotUIGameObject : MonoBehaviour
    {
        private Image BackgroundImage;
        private Material SkillIconBackgroundMaterialInstance;
        public SkillSlotUIconIGameObject SkillSlotUIconIGameObject { get; private set; }
        private SkillSlitUIInputTextGameObject SkillSlitUIInputTextGameObject;

        private Vector3 WorldCenter;

        public void Init()
        {
            this.BackgroundImage = GetComponentInChildren<Image>();

            /// We clone the material to allow independency between skill slots
            this.BackgroundImage.material = new Material(this.BackgroundImage.material);
            this.BackgroundImage.gameObject.AddComponent<PushUndistortedUVtoUV1>();

            this.SkillIconBackgroundMaterialInstance = this.BackgroundImage.material;
            this.SkillSlotUIconIGameObject = this.gameObject.GetComponentInChildren<SkillSlotUIconIGameObject>();
            this.SkillSlotUIconIGameObject.Init();

            this.SkillSlitUIInputTextGameObject = this.gameObject.GetComponentInChildren<SkillSlitUIInputTextGameObject>();
            this.SkillSlitUIInputTextGameObject.Init();
        }

        public void SetCooldownProgress(float cooldownProgress)
        {
            this.SkillIconBackgroundMaterialInstance.SetFloat("_CooldownProgress", cooldownProgress);
        }

        public void SetUIIcon(Sprite Icon)
        {
            this.SkillSlotUIconIGameObject.SetUIIcon(Icon);
        }

        public void SetInputText(string text)
        {
            this.SkillSlitUIInputTextGameObject.SetText(text);
        }

        public void PositionAndScaleSkillSlot(ref SKillSlotUIPositionInput SKillSlotUIPositionInput)
        {
            (this.transform as RectTransform).Reset(SKillSlotUIPositionInput.RootPivot);
            (this.transform as RectTransform).sizeDelta = SKillSlotUIPositionInput.RootSize;
            (this.transform as RectTransform).SetPivot(SKillSlotUIPositionInput.RootPivot);
            (this.transform as RectTransform).SetLocalPositionRelativeToCanvasScaler(SKillSlotUIPositionInput.RootLocalPositionInPercentage);

            (this.BackgroundImage.transform as RectTransform).sizeDelta = SKillSlotUIPositionInput.BackgroundImageSize;
            (this.SkillSlotUIconIGameObject.transform as RectTransform).sizeDelta = SKillSlotUIPositionInput.SlotIconSize;

            this.WorldCenter = (this.transform as RectTransform).position;
        }
    }

    public struct SKillSlotUIPositionInput
    {
        public RectTransformSetup RootPivot;
        public Vector2 RootSize;
        public Vector2 RootLocalPositionInPercentage;

        public Vector2 BackgroundImageSize;
        public Vector2 SlotIconSize;

        public SKillSlotUIPositionInput(RectTransformSetup rootPivot, Vector2 rootSize, Vector2 rootLocalPositionInPercentage, Vector2 backgroundImageSize, Vector2 slotIconSize)
        {
            RootPivot = rootPivot;
            RootSize = rootSize;
            RootLocalPositionInPercentage = rootLocalPositionInPercentage;
            BackgroundImageSize = backgroundImageSize;
            SlotIconSize = slotIconSize;
        }
    }
}