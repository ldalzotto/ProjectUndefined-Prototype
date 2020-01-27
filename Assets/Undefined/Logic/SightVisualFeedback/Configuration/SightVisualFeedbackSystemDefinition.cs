using System;
using AnimatorPlayable;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Serialization;

namespace SightVisualFeedback
{
    [Serializable]
    public class SightVisualFeedbackSystemDefinition : SerializedScriptableObject
    {
        public GameObject BaseAIStateIconPrefab;
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public A_AnimationPlayableDefinition SightVisualFeedbackAnimation;
        public Material WarningIconMaterial;
        public Material DangerIconMaterial;
    }
}