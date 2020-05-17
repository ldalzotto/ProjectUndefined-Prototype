using OdinSerializer;
using UnityEngine;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public abstract class AbstractInteractiveObjectV2Definition : SerializedScriptableObject
    {
        public abstract CoreInteractiveObject BuildInteractiveObject(GameObject parent);
    }
}