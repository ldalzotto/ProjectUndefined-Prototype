using AnimatorPlayable;
using CoreGame;
using InteractiveObjects;
using UnityEngine;
using UnityEngine.Playables;

namespace PlayerAim
{
    public class InteractiveObjectTargettedVisualFeedbackGameObject
    {
        public GameObject interactiveObjectTargettedVisualFeedbackObject;
        public Animator animator;

        public InteractiveObjectTargettedVisualFeedbackGameObject(GameObject parent, GameObject prefab)
        {
            this.interactiveObjectTargettedVisualFeedbackObject = GameObject.Instantiate(prefab);
            if (parent != null)
            {
                this.interactiveObjectTargettedVisualFeedbackObject.transform.parent = parent.transform;
            }

            this.interactiveObjectTargettedVisualFeedbackObject.transform.ResetLocal();

            this.animator = this.interactiveObjectTargettedVisualFeedbackObject.GetComponent<Animator>();
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            this.interactiveObjectTargettedVisualFeedbackObject.transform.position = worldPosition;
        }

        public void SetWorldRotation(Quaternion worldRotation)
        {
            this.interactiveObjectTargettedVisualFeedbackObject.transform.rotation = worldRotation;
        }
    }

    public struct InteractiveObjectTargettedVisualFeedbackObjectDefinition
    {
        public A_AnimationPlayableDefinition InteractiveObjectTargettedVisualFeedbackObjectAnimation;

        public InteractiveObjectTargettedVisualFeedbackObjectDefinition(A_AnimationPlayableDefinition interactiveObjectTargettedVisualFeedbackObjectAnimation)
        {
            InteractiveObjectTargettedVisualFeedbackObjectAnimation = interactiveObjectTargettedVisualFeedbackObjectAnimation;
        }
    }

    public class InteractiveObjectTargettedVisualFeedbackObject
    {
        public InteractiveObjectTargettedVisualFeedbackGameObject InteractiveObjectTargettedVisualFeedbackGameObject;

        public InteractiveObjectTargettedVisualFeedbackObject(InteractiveObjectTargettedVisualFeedbackGameObject InteractiveObjectTargettedVisualFeedbackGameObject,
            InteractiveObjectTargettedVisualFeedbackObjectDefinition InteractiveObjectTargettedVisualFeedbackObjectDefinition)
        {
            this.InteractiveObjectTargettedVisualFeedbackGameObject = InteractiveObjectTargettedVisualFeedbackGameObject;
            this.AnimatorPlayable = new AnimatorPlayableObject("InteractiveObjectTargettedVisualFeedbackObject", this.InteractiveObjectTargettedVisualFeedbackGameObject.animator);
            this.AnimatorPlayable.PlayAnimation(0, InteractiveObjectTargettedVisualFeedbackObjectDefinition.InteractiveObjectTargettedVisualFeedbackObjectAnimation.GetAnimationInput());
            this.AnimatorPlayable.GlobalPlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.UnscaledGameTime);
        }

        private AnimatorPlayableObject AnimatorPlayable;

        public void Tick(float d)
        {
            this.AnimatorPlayable.Tick(d);
        }

        public void TickTimeFrozen()
        {
            this.Tick(0f);
        }

        public void Destroy()
        {
            this.AnimatorPlayable.Destroy();
            GameObject.Destroy(this.InteractiveObjectTargettedVisualFeedbackGameObject.interactiveObjectTargettedVisualFeedbackObject);
        }
    }
}