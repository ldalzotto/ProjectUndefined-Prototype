using System;
using AnimatorPlayable;
using OdinSerializer;

namespace SoldierAnimation
{
    [Serializable]
    public class SoldierAnimationSystemDefinition : SerializedScriptableObject
    {
        [Inline()] public A_AnimationPlayableDefinition FiringPoseAnimation;
    }
}