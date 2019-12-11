using System;
using AnimatorPlayable;

namespace UnityEngine.Rendering
{
    public class TreeAnimationTest : MonoBehaviour
    {
        private AnimatorPlayableObject AnimatorPlayableObject;
        public Vector2 Input;
        public TwoDBlendTreePlayableDefinition TwoDBlendTreePlayableDefinition;

        private void Start()
        {
            this.AnimatorPlayableObject = new AnimatorPlayableObject("Test", this.GetComponent<Animator>());
            this.AnimatorPlayableObject.PlayAnimation(0, TwoDBlendTreePlayableDefinition.GetAnimationInput(), null, null);
        }

        private void Update()
        {
            this.AnimatorPlayableObject.SetTwoDInputWeight(0, this.Input);
            this.AnimatorPlayableObject.Tick(Time.deltaTime);
        }

    }
}