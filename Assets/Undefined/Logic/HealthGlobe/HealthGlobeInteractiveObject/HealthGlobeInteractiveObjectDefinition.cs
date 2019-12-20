using System;
using Health;
using InteractiveObjects;
using OdinSerializer;
using UnityEngine;

namespace HealthGlobe
{
    [Serializable]
    public class HealthGlobeInteractiveObjectDefinition : AbstractInteractiveObjectV2Definition
    {
        [Inline(CreateAtSameLevelIfAbsent = true)]
        public RecoveringHealthEmitterSystemDefinition RecoveringHealthEmitterSystemDefinition;

        public override CoreInteractiveObject BuildInteractiveObject(GameObject interactiveGameObject)
        {
            return new HealthGlobeInteractiveObject(this, InteractiveGameObjectFactory.Build(interactiveGameObject));
        }
    }
}