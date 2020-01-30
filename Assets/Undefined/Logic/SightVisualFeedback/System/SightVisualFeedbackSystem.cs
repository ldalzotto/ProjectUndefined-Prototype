using CoreGame;
using InteractiveObjects;
using UnityEngine;

namespace SightVisualFeedback
{
    public enum SightVisualFeedbackColorType
    {
        NONE = 0,
        NEUTRAL = 1,
        WARNING = 2,
        DANGER = 3
    }

    /// <summary>
    /// The <see cref="SightVisualFeedbackSystem"/> is responsible of :
    ///     - Displaying the corrent icon base on <see cref="SightVisualFeedbackColorType"/>. The icon object is <see cref="SightVisualFeedbackSystemDefinition"/>.
    ///     - Making the <see cref="SightVisualFeedbackSystemDefinition"/> facing the camera.
    /// </summary>
    public struct SightVisualFeedbackSystem
    {
        private SightVisualFeedbackSystemDefinitionPointer SightVisualFeedbackSystemDefinitionPtr;
        private CoreInteractiveObjectPointer AssociatedInteractiveObjectPtr;
        private CameraPointer MainCameraPtr;

        public void Initialize(SightVisualFeedbackSystemDefinition sightVisualFeedbackSystemDefinition, CoreInteractiveObject associatedInteractiveObject, Camera MainCamera)
        {
            SightVisualFeedbackSystemDefinitionPtr = sightVisualFeedbackSystemDefinition.ToPointer();
            this.AssociatedInteractiveObjectPtr = associatedInteractiveObject.ToPointer();
            this.MainCameraPtr = MainCamera.ToPointer();
            this.SightVisualFeedbackGameObjectV2 =
                new SightVisualFeedbackGameObjectV2(GameObject.Instantiate(sightVisualFeedbackSystemDefinition.BaseAIStateIconPrefab,
                    associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform), sightVisualFeedbackSystemDefinition.SightVisualFeedbackAnimation);
        }

        private SightVisualFeedbackGameObjectV2 SightVisualFeedbackGameObjectV2;


        public void AfterTicks(float d)
        {
            this.SightVisualFeedbackGameObjectV2.AfterTicks(d, this.MainCameraPtr.GetValue());
        }

        public void TickTimeFrozen(float d)
        {
            this.AfterTicks(d);
        }

        public void Destroy()
        {
            this.SightVisualFeedbackGameObjectV2.Destroy();
        }

        public void Show(SightVisualFeedbackColorType SightVisualFeedbackColorType)
        {
            switch (SightVisualFeedbackColorType)
            {
                case SightVisualFeedbackColorType.WARNING:
                    this.SightVisualFeedbackGameObjectV2.SetMaterial(this.SightVisualFeedbackSystemDefinitionPtr.GetValue().WarningIconMaterial);
                    this.SightVisualFeedbackGameObjectV2.SetWorldPosition(this.GetSightVisualFeedbackSystemDefinitionWorldPosition());
                    break;
                case SightVisualFeedbackColorType.DANGER:
                    this.SightVisualFeedbackGameObjectV2.SetMaterial(this.SightVisualFeedbackSystemDefinitionPtr.GetValue().DangerIconMaterial);
                    this.SightVisualFeedbackGameObjectV2.SetWorldPosition(this.GetSightVisualFeedbackSystemDefinitionWorldPosition());
                    break;
            }

            this.SightVisualFeedbackGameObjectV2.SetActive(true);
        }

        public void Hide()
        {
            this.SightVisualFeedbackGameObjectV2.SetActive(false);
        }

        private Vector3 GetSightVisualFeedbackSystemDefinitionWorldPosition()
        {
            return this.AssociatedInteractiveObjectPtr.GetValue().InteractiveGameObject.GetAverageModelWorldBounds().center
                   + new Vector3(0, this.AssociatedInteractiveObjectPtr.GetValue().InteractiveGameObject.GetAverageModelWorldBounds().max.y * 0.65f, 0);
        }
    }
}