using System;
using OdinSerializer;

namespace GeometryIntersection
{
    [Serializable]
    public class VisibilityProbeDefinition : SerializedScriptableObject
    {
        /// <summary>
        /// Tells the visibility probe generation algorithm (<see cref="VisibilityProbeGeneration"/>) the density of generated probes.
        /// </summary>
        public float ProbeDensityPerUnit;
    }
}