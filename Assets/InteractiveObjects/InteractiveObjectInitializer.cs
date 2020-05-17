using System;
using UnityEngine;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    [Serializable]
    public class InteractiveObjectInitializer : MonoBehaviour
    {
        [CustomEnum(ConfigurationType = typeof(InteractiveObjectV2Configuration))] [DrawConfiguration(ConfigurationType = typeof(InteractiveObjectV2Configuration))]
        public InteractiveObjectV2DefinitionID InteractiveObjectV2DefinitionID;

        public virtual void Init()
        {
            InteractiveObjectV2ConfigurationGameObject.Get().InteractiveObjectV2Configuration.ConfigurationInherentData[InteractiveObjectV2DefinitionID].BuildInteractiveObject(gameObject);
        }
    }
}