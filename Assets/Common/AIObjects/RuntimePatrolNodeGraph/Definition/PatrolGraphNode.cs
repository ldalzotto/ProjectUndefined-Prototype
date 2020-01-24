using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEngine;

namespace AIObjects
{
    [Serializable]
    public class PatrolGraphNode : SerializedScriptableObject
    {
        public Vector3 WorldPosition;

        [HideInInspector] public bool WorldRotationEnabled;

        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(WorldRotationEnabled))]
        public Vector3 WorldRotation;

        [HideInInspector] public bool IsWaiting;

        [Foldable(canBeDisabled: true, disablingBoolAttribute: nameof(IsWaiting))]
        public float WaitTime;

        public List<PatrolGraphNodeLink> Links;
    }
}