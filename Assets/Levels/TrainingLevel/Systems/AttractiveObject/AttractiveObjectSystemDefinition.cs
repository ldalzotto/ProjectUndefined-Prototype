using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace InteractiveObjects
{
    [System.Serializable]
    [SceneHandleDraw]
    public class AttractiveObjectSystemDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 1, G = 1, B = 0)]
        public float EffectRange;

        public float EffectiveTime;
    }

    public class AIAttractiveObjectState
    {
        public CoreInteractiveObject AttractedInteractiveObject { get; private set; }
        public BoolVariable IsAttractedByAttractiveObject { get; private set; }

        public AIAttractiveObjectState(BoolVariable isAttractedByAttractiveObject)
        {
            IsAttractedByAttractiveObject = isAttractedByAttractiveObject;
        }

        public void SetIsAttractedByAttractiveObject(bool value, CoreInteractiveObject AttractedInteractiveObject)
        {
            this.AttractedInteractiveObject = AttractedInteractiveObject;
            this.IsAttractedByAttractiveObject.SetValue(value);
        }
    }
}
