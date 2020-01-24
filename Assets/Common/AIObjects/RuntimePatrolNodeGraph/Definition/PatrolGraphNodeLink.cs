using System;
using InteractiveObjects_Interfaces;
using OdinSerializer;
using UnityEngine.Serialization;

namespace AIObjects
{
    [Serializable]
    public class PatrolGraphNodeLink : SerializedScriptableObject
    {
        public AIMovementSpeedAttenuationFactor AIMovementSpeed;
        [FormerlySerializedAs("PatrolGraphNode2")] public PatrolGraphNode PatrolGraphNode;
    }
}