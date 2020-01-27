using System.Runtime.InteropServices;
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
    public class SightVisualFeedbackSystem
    {
        private SightVisualFeedbackSystemDefinition SightVisualFeedbackSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;
        private Camera MainCamera;

        public SightVisualFeedbackSystem(SightVisualFeedbackSystemDefinition sightVisualFeedbackSystemDefinition, CoreInteractiveObject associatedInteractiveObject, Camera MainCamera)
        {
            SightVisualFeedbackSystemDefinition = sightVisualFeedbackSystemDefinition;
            this.AssociatedInteractiveObject = associatedInteractiveObject;
            this.MainCamera = MainCamera;
            this.SightVisualFeedbackGameObject =
                new SightVisualFeedbackGameObject(GameObject.Instantiate(this.SightVisualFeedbackSystemDefinition.BaseAIStateIconPrefab,
                    this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform), this.SightVisualFeedbackSystemDefinition.SightVisualFeedbackAnimation);
        }


        private SightVisualFeedbackGameObject SightVisualFeedbackGameObject;

        public void AfterTicks(float d)
        {
            this.SightVisualFeedbackGameObject.AssociatedGameObject.transform.rotation = Quaternion.LookRotation(-this.MainCamera.transform.forward);
            this.SightVisualFeedbackGameObject.AfterTicks(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.AfterTicks(d);
        }

        public void Destroy()
        {
            if (this.SightVisualFeedbackGameObject != null)
            {
                this.SightVisualFeedbackGameObject.Destroy();
            }
        }

        public void Show(SightVisualFeedbackColorType SightVisualFeedbackColorType)
        {
            switch (SightVisualFeedbackColorType)
            {
                case SightVisualFeedbackColorType.WARNING:
                    this.SightVisualFeedbackGameObject.SetMaterial(this.SightVisualFeedbackSystemDefinition.WarningIconMaterial);
                    this.SightVisualFeedbackGameObject.AssociatedGameObject.transform.position = this.GetSightVisualFeedbackSystemDefinitionWorldPosition();
                    break;
                case SightVisualFeedbackColorType.DANGER:
                    this.SightVisualFeedbackGameObject.SetMaterial(this.SightVisualFeedbackSystemDefinition.DangerIconMaterial);
                    this.SightVisualFeedbackGameObject.AssociatedGameObject.transform.position = this.GetSightVisualFeedbackSystemDefinitionWorldPosition();
                    break;
            }

            this.SightVisualFeedbackGameObject.AssociatedGameObject.SetActive(true);
        }

        public void Hide()
        {
            this.SightVisualFeedbackGameObject.AssociatedGameObject.SetActive(false);
        }

        private Vector3 GetSightVisualFeedbackSystemDefinitionWorldPosition()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().center + new Vector3(0, this.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().max.y * 0.65f, 0);
        }
    }
}