using System;
using GeometryIntersection;
using OdinSerializer;

namespace InteractiveObjects
{
    [Serializable]
    [SceneHandleDraw]
    public class SightObjectSystemDefinition : SerializedScriptableObject
    {
        [WireRoundedFrustum(R = 1f, G = 1f, B = 0f)]
        public FrustumV2 Frustum;
    }
}