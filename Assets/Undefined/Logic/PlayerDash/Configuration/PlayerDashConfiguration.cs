using System;
using OdinSerializer;
using UnityEngine;

namespace PlayerDash
{
    [Serializable]
    public class PlayerDashConfiguration : SerializedScriptableObject
    {
        public LineRenderer DashPathVisualFeedbackPrefab;
    }
}