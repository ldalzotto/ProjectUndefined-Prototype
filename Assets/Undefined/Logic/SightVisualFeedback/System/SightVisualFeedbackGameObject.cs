using System;
using AnimatorPlayable;
using CoreGame;
using UnityEngine;
using UnityEngine.Playables;

namespace SightVisualFeedback
{
    public struct SightVisualFeedbackGameObjectV2 : IDisposable
    {
        private GameObjectPointer AssociatedGameObjectPtr;
        private MeshRendererPointer MeshRendererPtr;
        private AnimatorPlayableObjectPointer AnimatorPlayableObjectPtr;

        public SightVisualFeedbackGameObjectV2(GameObject AssociatedGameObject,
            A_AnimationPlayableDefinition SightVisualFeedbackAnimation)
        {
            this.AssociatedGameObjectPtr = AssociatedGameObject.Allocate();
            this.MeshRendererPtr = AssociatedGameObject.GetComponentInChildren<MeshRenderer>().Allocate();
            this.AnimatorPlayableObjectPtr = new AnimatorPlayableObject("SightVisualFeedbackGameObject", AssociatedGameObject.GetComponent<Animator>()).Allocate();
            this.AnimatorPlayableObjectPtr.GetValue().PlayAnimation(0, SightVisualFeedbackAnimation.GetAnimationInput());
            this.AnimatorPlayableObjectPtr.GetValue().GlobalPlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.UnscaledGameTime);
        }

        public void AfterTicks(float d, Camera MainCamera)
        {
            this.AssociatedGameObjectPtr.GetValue().transform.rotation = Quaternion.LookRotation(-MainCamera.transform.forward);
            this.AnimatorPlayableObjectPtr.GetValue().Tick(d);
        }

        public void SetMaterial(Material material)
        {
            this.MeshRendererPtr.GetValue().material.CopyPropertiesFromMaterial(material);
        }

        public void SetWorldPosition(Vector3 WorldPoisition)
        {
            this.AssociatedGameObjectPtr.GetValue().transform.position = WorldPoisition;
        }

        public void SetActive(bool IsActive)
        {
            this.AssociatedGameObjectPtr.GetValue().SetActive(IsActive);
        }

        public void Destroy()
        {
            GameObject.Destroy(this.AssociatedGameObjectPtr.GetValue());
            this.Dispose();
        }

        public void Dispose()
        {
            AssociatedGameObjectPtr.Dispose();
            MeshRendererPtr.Dispose();
            AnimatorPlayableObjectPtr.Dispose();
        }
    }
}