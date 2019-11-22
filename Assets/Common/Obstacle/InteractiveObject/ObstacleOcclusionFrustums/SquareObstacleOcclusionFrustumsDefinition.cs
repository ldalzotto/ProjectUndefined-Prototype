using System.Collections.Generic;
using CoreGame;
using GeometryIntersection;
using InteractiveObjects_Interfaces;
using UnityEngine;

namespace Obstacle
{
    internal class SquareObstacleOcclusionFrustumsDefinition
    {
        public SquareObstacleOcclusionFrustumsDefinition(InteractiveObjectBoxLogicColliderDefinition InteractiveObjectBoxLogicColliderDefinition)
        {
            FaceFrustums = new List<FrustumV2>();

            Intersection.ExtractBoxColliderLocalPoints(InteractiveObjectBoxLogicColliderDefinition.LocalCenter, InteractiveObjectBoxLogicColliderDefinition.LocalSize, 
                out Vector3 lC1, out Vector3 lC2, out Vector3 lC3, out Vector3 lC4, out Vector3 lC5,
                out Vector3 lC6, out Vector3 lC7, out Vector3 lC8);
            
            //Create frustum for all sides -> occlusions are only calculated for facing frustums.
            /// Normals resulted by cross product must be directed inside the cube 
            this.CreateAndAddFrustumV2(lC2, lC1, lC4, lC3);
            this.CreateAndAddFrustumV2(lC5, lC6, lC7, lC8);
            
            this.CreateAndAddFrustumV2(lC1, lC2, lC6, lC5);
            this.CreateAndAddFrustumV2(lC4, lC8, lC7, lC3);
            
            this.CreateAndAddFrustumV2(lC2, lC3, lC7, lC6);
            this.CreateAndAddFrustumV2(lC1, lC5, lC8, lC4);
        }

        public List<FrustumV2> FaceFrustums { get; private set; }

        private void CreateAndAddFrustumV2(Vector3 lC1, Vector3 lC2, Vector3 lC3, Vector3 lC4)
        {
            var frustum = new FrustumV2();
            frustum.FaceDistance = 9999f;
            frustum.F2v3 = new FrustumFaceV3();
            frustum.F1v3.LocalC1 = lC1;
            frustum.F1v3.LocalC2 = lC2;
            frustum.F1v3.LocalC3 = lC3;
            frustum.F1v3.LocalC4 = lC4;
            FaceFrustums.Add(frustum);
            
        }
    }
}