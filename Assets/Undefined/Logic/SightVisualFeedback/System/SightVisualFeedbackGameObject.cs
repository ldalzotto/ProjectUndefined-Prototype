using AnimatorPlayable;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace SightVisualFeedback
{
    public class SightVisualFeedbackGameObject
    {
        public GameObject AssociatedGameObject;
        private MeshRenderer MeshRenderer;
        private AnimatorPlayableObject AnimatorPlayableObject;

        public SightVisualFeedbackGameObject(GameObject AssociatedGameObject,
            A_AnimationPlayableDefinition SightVisualFeedbackAnimation)
        {
            this.AssociatedGameObject = AssociatedGameObject;
            this.MeshRenderer = this.AssociatedGameObject.GetComponentInChildren<MeshRenderer>();
            this.AnimatorPlayableObject = new AnimatorPlayableObject("SightVisualFeedbackGameObject", this.AssociatedGameObject.GetComponent<Animator>());
            this.AnimatorPlayableObject.PlayAnimation(0, SightVisualFeedbackAnimation.GetAnimationInput());
            this.AnimatorPlayableObject.GlobalPlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.UnscaledGameTime);
        }

        public void AfterTicks(float d)
        {
            this.AnimatorPlayableObject.Tick(d);
        }

        public void SetMaterial(Material material)
        {
            this.MeshRenderer.material.CopyPropertiesFromMaterial(material);
        }

        public void Destroy()
        {
            this.AnimatorPlayableObject.Destroy();
            GameObject.Destroy(this.AssociatedGameObject);
        }
    }
}