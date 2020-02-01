using System;
using GeometryIntersection;
using UnityEngine;

namespace Common.GeometryIntersection.Test
{
    public class VisibilityProbeGenerationTest : MonoBehaviour
    {
        public float ProbeDensityPerUnit;
        public BoxCollider B1;

        private VisibilityProbe VisibilityProbe;

        private void Start()
        {
            this.VisibilityProbe = VisibilityProbeGeneration.GenerateAndAlocateVisibilityProbeLocalPointsFrom(ProbeDensityPerUnit, new BoxDefinition(this.B1));
        }

        private void OnDestroy()
        {
        }

        private void OnDrawGizmos()
        {
            int size = this.VisibilityProbe.GetSize();
            if (size > 0)
            {
                var BoxDefinition = new BoxDefinition(this.B1);
                for (int i = 0; i < size; i++)
                {
                    Gizmos.DrawWireSphere(
                        BoxDefinition.LocalToWorld.MultiplyPoint(this.VisibilityProbe[i]), 0.1f);
                }
            }
        }
    }
}