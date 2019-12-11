using AnimatorPlayable;
using UnityEngine;

#if UNITY_EDITOR
namespace InteractiveObject_Animation
{
    public class TwoDObjectAnimationPlayableSystem
    {
        private AnimatorPlayableObject AnimatorPlayableObjectRef;

        public TwoDObjectAnimationPlayableSystem(AnimatorPlayableObject AnimatorPlayableObject, A_AnimationPlayableDefinition Blendtree)
        {
            this.AnimatorPlayableObjectRef = AnimatorPlayableObject;
            AnimatorPlayableObject.PlayAnimation(0, Blendtree.GetAnimationInput());
        }

        public void SetCurrentSpeed(Vector2 CurrentSpeed)
        {
            this.AnimatorPlayableObjectRef.SetTwoDInputWeight(0, CurrentSpeed);
        }
    }
}
#endif