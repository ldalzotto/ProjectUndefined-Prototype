using AnimatorPlayable;
using UnityEngine;

#if UNITY_EDITOR
namespace InteractiveObject_Animation
{
    public class TwoDObjectAnimationPlayableSystem
    {
        private Vector2 CurrentSpeed;

        public TwoDObjectAnimationPlayableSystem(AnimatorPlayableObject AnimatorPlayableObject, A_AnimationPlayableDefinition Blendtree)
        {
            AnimatorPlayableObject.PlayAnimation(0, Blendtree.GetAnimationInput(),
                TwoDInputWheigtProvider: () => this.CurrentSpeed);
        }

        public void SetCurrentSpeed(Vector2 CurrentSpeed)
        {
            this.CurrentSpeed = CurrentSpeed;
        }
    }
}
#endif