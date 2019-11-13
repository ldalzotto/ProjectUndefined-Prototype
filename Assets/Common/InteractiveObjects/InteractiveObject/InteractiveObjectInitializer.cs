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

        public CoreInteractiveObject Init()
        {
            this.CreatedCoreInteractiveObject = this.InitializationLogic();
            MonoBehaviour.Destroy(this);
            return this.CreatedCoreInteractiveObject;
        }

        /// <summary>
        /// Overriding this method allow to redefine the InitializationLogic while keeping common behaviors defined in <see cref="Init"/>
        /// </summary>
        protected virtual CoreInteractiveObject InitializationLogic()
        {
            return this.AbstractInteractiveObjectV2Definition.BuildInteractiveObject(gameObject);
        }
    }
}