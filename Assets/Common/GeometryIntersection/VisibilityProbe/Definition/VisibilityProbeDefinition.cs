using System;
using OdinSerializer;

namespace GeometryIntersection
{
    [Serializable]
    public class VisibilityProbeDefinition : SerializedScriptableObject
    {
        public float ProbeDensityPerUnit;
    }
}