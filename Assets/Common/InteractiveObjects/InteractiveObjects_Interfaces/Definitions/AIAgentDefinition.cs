using System;
using OdinSerializer;

namespace InteractiveObjects_Interfaces
{
    [Serializable]
    [SceneHandleDraw]
    public class AIAgentDefinition : SerializedScriptableObject
    {
        [WireDirectionalLineAttribute(R = 0f, G = 1f, B = 0f, dY = 1f)]
        public float AgentHeight = 2f;

        [WireCircle(R = 1f, G = 0f, B = 0F)] public float AgentRadius = 0.5f;
        [WireCircle(R = 0f, G = 1f, B = 0f)] public float AgentStoppingDistance = 0.5f;
    }
}