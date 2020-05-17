using System;
using OdinSerializer;
using RangeObjects;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public class LevelCompletionZoneSystemDefinition : SerializedScriptableObject
    {
        [DrawNested] [Inline(true)] public RangeObjectInitialization TriggerRangeObjectInitialization;
    }
}