using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace Obstacle
{
    internal class SquareObstacleOcclusionFrustumsDefinition
    {
        public SquareObstacleOcclusionFrustumsDefinition()
        {
            FaceFrustums = new List<FrustumV2>();

            //Create frustum for all sides -> occlusions are only calculated for facing frustums.
            CreateAndAddFrustum(new Vector3(0, 0, 0), 1);
            CreateAndAddFrustum(new Vector3(0, 180, 0), 1);
            CreateAndAddFrustum(new Vector3(0, 90, 0), 1);
            CreateAndAddFrustum(new Vector3(0, -90, 0), 1);
            CreateAndAddFrustum(new Vector3(90, 0, 0), 1);
            CreateAndAddFrustum(new Vector3(-90, 0, 0), 1);
        }

        public List<FrustumV2> FaceFrustums { get; private set; }

        private void CreateAndAddFrustum(Vector3 deltaRotationEuler, float F1FaceZOffset)
        {
            var frustum = new FrustumV2();
            frustum.FaceDistance = 9999f;
            frustum.F1.Width = 1f;
            frustum.F1.Height = 1f;
            frustum.DeltaRotation = deltaRotationEuler;
            frustum.F1.FaceOffsetFromCenter.z = F1FaceZOffset;
            FaceFrustums.Add(frustum);
        }
    }
}