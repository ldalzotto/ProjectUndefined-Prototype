using System;
using OdinSerializer;
using UnityEngine;

namespace FiredProjectile
{
    [Serializable]
    public class PlayerAimingRangeVisualFeedbackConfiguration : SerializedScriptableObject
    {
        public GameObject FiredProjectileFeedbackPrefab;
    }
}