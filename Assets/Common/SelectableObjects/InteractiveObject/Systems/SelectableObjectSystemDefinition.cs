using System;
using OdinSerializer;

namespace SelectableObject
{
    [Serializable]
    [SceneHandleDraw]
    public class SelectableObjectSystemDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 0f, G = 0f, B = 1f)] public float SelectionRange;
    }
}