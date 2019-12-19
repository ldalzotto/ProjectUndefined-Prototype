using InteractiveObject_Animation;
using InteractiveObjects;
using UnityEngine;

namespace PlayerObject
{
    public struct PlayerVisualEffectSystem
    {
        private PlayerOnLowHealthVisualEffectSystem PlayerOnLowHealthVisualEffectSystem;

        public PlayerVisualEffectSystem(CoreInteractiveObject AssociatedInteractiveObject,
            PlayerVisualEffectSystemDefinition PlayerVisualEffectSystemDefinition)
        {
            this.PlayerOnLowHealthVisualEffectSystem = new PlayerOnLowHealthVisualEffectSystem(AssociatedInteractiveObject, PlayerVisualEffectSystemDefinition);
        }

        public void LateTick(float d)
        {
            this.PlayerOnLowHealthVisualEffectSystem.LateTick(d);
        }

        public void OnLowHealthStarted()
        {
            this.PlayerOnLowHealthVisualEffectSystem.OnLowHealthStarted();
        }

        public void OnLowHealthEnded()
        {
            this.PlayerOnLowHealthVisualEffectSystem.OnLowHealthEnded();
        }
    }

    struct PlayerOnLowHealthVisualEffectSystem
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private PlayerVisualEffectSystemDefinition PlayerVisualEffectSystemDefinition;

        private PlayerOnLowHealthVisualEffectComponent PlayerOnLowHealthVisualEffectComponent;

        private Renderer PlayerBodyRenderer;

        private bool IsOnLowHealth;

        public PlayerOnLowHealthVisualEffectSystem(CoreInteractiveObject AssociatedInteractiveObject, PlayerVisualEffectSystemDefinition PlayerVisualEffectSystemDefinition) : this()
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.PlayerVisualEffectSystemDefinition = PlayerVisualEffectSystemDefinition;
            this.PlayerBodyRenderer = this.AssociatedInteractiveObject.InteractiveGameObject.Animator.gameObject.FindChildObjectRecursively("Body").GetComponent<Renderer>();

            this.ResetPlayerEmissionColor();
        }

        public void LateTick(float d)
        {
            if (this.IsOnLowHealth)
            {
                var materials = this.PlayerBodyRenderer.materials;
                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i].SetVector("_EmissionColor", materials[i].GetVector("_EmissionColor").normalized * this.PlayerOnLowHealthVisualEffectComponent.EmissionIntensity);
                }
            }
        }

        public void OnLowHealthStarted()
        {
            this.PlayerOnLowHealthVisualEffectComponent = this.AssociatedInteractiveObject.InteractiveGameObject.Animator.gameObject.AddComponent<PlayerOnLowHealthVisualEffectComponent>();
            this.AssociatedInteractiveObject.AnimationController.PlayAnimationV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOW_HEALTH_VISUAL_EFFET),
                this.PlayerVisualEffectSystemDefinition.OnLowHealthVisualFeedbackAnimation.GetAnimationInput());
            this.IsOnLowHealth = true;
        }

        public void OnLowHealthEnded()
        {
            MonoBehaviour.Destroy(this.PlayerOnLowHealthVisualEffectComponent);
            this.AssociatedInteractiveObject.AnimationController.DestroyAnimationLayerV2(PlayerObjectAnimationLayersOrders.GetLayerNumber(PlayerObjectAnimationLayers.LOW_HEALTH_VISUAL_EFFET));
            this.IsOnLowHealth = false;
            this.ResetPlayerEmissionColor();
        }

        private void ResetPlayerEmissionColor()
        {
            var materials = this.PlayerBodyRenderer.materials;
            for (var i = 0; i < materials.Length; i++)
            {
                materials[i].SetVector("_EmissionColor", materials[i].GetVector("_EmissionColor").normalized);
            }
        }
    }
}