using System;
using OdinSerializer;
using RangeObjects;
using UnityEngine;

namespace HealthGlobe
{
    [Serializable]
    public class GlobalHealthGlobeConfiguration : SerializedScriptableObject
    {
        public HealthGlobeInteractiveObjectInitializer HealthGlobeTemplatePrefab;
    }
}