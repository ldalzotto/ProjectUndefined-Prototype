using System;
using AnimatorPlayable;
using OdinSerializer;

namespace PlayerObject
{
    [Serializable]
    public class PlayerVisualEffectSystemDefinition : SerializedScriptableObject
    {
        public A_AnimationPlayableDefinition OnLowHealthVisualFeedbackAnimation;
    }
}