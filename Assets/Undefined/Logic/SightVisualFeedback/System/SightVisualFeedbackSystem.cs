using System.Runtime.InteropServices;
using InteractiveObjects;
using UnityEngine;

namespace SightVisualFeedback
{
    public class SightVisualFeedbackSystem
    {
        private SightVisualFeedbackSystemDefinition SightVisualFeedbackSystemDefinition;
        private CoreInteractiveObject AssociatedInteractiveObject;

        public SightVisualFeedbackSystem(SightVisualFeedbackSystemDefinition sightVisualFeedbackSystemDefinition, CoreInteractiveObject associatedInteractiveObject)
        {
            SightVisualFeedbackSystemDefinition = sightVisualFeedbackSystemDefinition;
            this.AssociatedInteractiveObject = associatedInteractiveObject;
            this.SightVisualFeedbackGameObject =
                new SightVisualFeedbackGameObject(GameObject.Instantiate(this.SightVisualFeedbackSystemDefinition.BaseAIStateIconPrefab,
                    this.AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform), this.SightVisualFeedbackSystemDefinition.SightVisualFeedbackAnimation);
        }

        private void OnCurrentFeedbakcIconChanged(GameObject old, GameObject newIcon)
        {
            if (old != null)
            {
                GameObject.Destroy(old);
            }
        }

        private SightVisualFeedbackGameObject SightVisualFeedbackGameObject;

        public void AfterTicks(float d)
        {
            this.SightVisualFeedbackGameObject.AssociatedGameObject.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward);
            this.SightVisualFeedbackGameObject.AfterTicks(d);
        }

        public void TickTimeFrozen(float d)
        {
            this.AfterTicks(d);
        }

        public void Destroy()
        {
        }

        public void Show(SightVisualFeedbackColorType SightVisualFeedbackColorType)
        {
            switch (SightVisualFeedbackColorType)
            {
                case SightVisualFeedbackColorType.WARNING:
                    this.SightVisualFeedbackGameObject.SetMaterial(this.SightVisualFeedbackSystemDefinition.WarningIconMaterial);
                    this.SightVisualFeedbackGameObject.AssociatedGameObject.transform.position = this.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().center + new Vector3(0, this.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().max.y * 0.65f, 0);
                    break;
                case SightVisualFeedbackColorType.DANGER:
                    this.SightVisualFeedbackGameObject.SetMaterial(this.SightVisualFeedbackSystemDefinition.DangerIconMaterial);
                    this.SightVisualFeedbackGameObject.AssociatedGameObject.transform.position = this.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().center + new Vector3(0, this.AssociatedInteractiveObject.InteractiveGameObject.GetAverageModelWorldBounds().max.y * 0.65f, 0);
                    break;
            }

            this.SightVisualFeedbackGameObject.AssociatedGameObject.SetActive(true);
        }

        public void Hide()
        {
            this.SightVisualFeedbackGameObject.AssociatedGameObject.SetActive(false);
        }
    }

    public enum SightVisualFeedbackColorType
    {
        NONE = 0,
        NEUTRAL = 1,
        WARNING = 2,
        DANGER = 3
    }

    public interface ISightVisualFeedbackPositioner
    {
        void Tick(float d);
    }

    struct CoreInteractiveObjectSightVisualFeedbackPositioner : ISightVisualFeedbackPositioner
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private CoreInteractiveObject LockedInteractiveObject;
        private LineRenderer InstanciatedLineVisualFeedback;

        public CoreInteractiveObjectSightVisualFeedbackPositioner(CoreInteractiveObject AssociatedInteractiveObject,
            CoreInteractiveObject lockedInteractiveObject, LineRenderer instanciatedVisualFeedbackRef)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            LockedInteractiveObject = lockedInteractiveObject;
            InstanciatedLineVisualFeedback = instanciatedVisualFeedbackRef;
        }

        public void Tick(float d)
        {
            if (this.LockedInteractiveObject != null && InstanciatedLineVisualFeedback.gameObject.activeSelf)
            {
                this.InstanciatedLineVisualFeedback.positionCount = 2;
                this.InstanciatedLineVisualFeedback.SetPosition(0, this.AssociatedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.AssociatedInteractiveObject.InteractiveGameObject.AverageModelLocalBounds.Bounds.center));
                this.InstanciatedLineVisualFeedback.SetPosition(1, this.LockedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.LockedInteractiveObject.InteractiveGameObject.AverageModelLocalBounds.Bounds.center));
            }
        }
    }

    struct WorldPositionSightVisualFeedbackPositioner : ISightVisualFeedbackPositioner
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private Vector3 LockedWorldPosition;
        private LineRenderer InstanciatedLineVisualFeedback;

        public WorldPositionSightVisualFeedbackPositioner(CoreInteractiveObject AssociatedInteractiveObject,
            Vector3 LockedWorldPosition, LineRenderer instanciatedVisualFeedbackRef)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.LockedWorldPosition = LockedWorldPosition;
            InstanciatedLineVisualFeedback = instanciatedVisualFeedbackRef;
        }

        public void Tick(float d)
        {
            if (InstanciatedLineVisualFeedback.gameObject.activeSelf)
            {
                this.InstanciatedLineVisualFeedback.positionCount = 2;
                this.InstanciatedLineVisualFeedback.SetPosition(0, this.AssociatedInteractiveObject.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(this.AssociatedInteractiveObject.InteractiveGameObject.AverageModelLocalBounds.Bounds.center));
                this.InstanciatedLineVisualFeedback.SetPosition(1, this.LockedWorldPosition);
            }
        }
    }
}