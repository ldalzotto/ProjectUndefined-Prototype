using System;
using OdinSerializer;
using UnityEngine;

namespace FiredProjectile
{
    [Serializable]
    public class FiringRangeVisualFeedbackConfiguration : SerializedScriptableObject
    {
        public GameObject FiredProjectileFeedbackPrefab;
    }
}