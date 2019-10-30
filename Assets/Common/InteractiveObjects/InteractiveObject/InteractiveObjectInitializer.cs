using System;
using UnityEngine;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    [Serializable]
    public class InteractiveObjectInitializer : MonoBehaviour
    {
        [Inline()] public AbstractInteractiveObjectV2Definition AbstractInteractiveObjectV2Definition;

        public virtual void Init()
        {
            this.AbstractInteractiveObjectV2Definition.BuildInteractiveObject(gameObject);
        }
    }
}