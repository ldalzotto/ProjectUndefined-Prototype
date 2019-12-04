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
            this.AnimatorPlayableObject.PlayAnimation(0, TwoDBlendTreePlayableDefinition.GetAnimationInput(), null, null, TwoDInputWheigtProvider);
        }

        private void Update()
        {
            this.AnimatorPlayableObject.Tick(Time.deltaTime);
        }

        private Vector2 TwoDInputWheigtProvider()
        {
            return this.Input;
        }
    }
}