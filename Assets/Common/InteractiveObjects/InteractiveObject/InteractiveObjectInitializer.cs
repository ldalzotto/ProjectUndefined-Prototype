using System;
using UnityEngine;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    [Serializable]
    public class InteractiveObjectInitializer : MonoBehaviour
    {
        [Inline()] [DrawNested] public AbstractInteractiveObjectV2Definition AbstractInteractiveObjectV2Definition;

        protected CoreInteractiveObject CreatedCoreInteractiveObject;

        public virtual CoreInteractiveObject Init()
        {
            this.CreatedCoreInteractiveObject = this.AbstractInteractiveObjectV2Definition.BuildInteractiveObject(gameObject);
            return this.CreatedCoreInteractiveObject;
        }
    }
}