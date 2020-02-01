using CoreGame;
using UnityEngine;

namespace GeometryIntersection
{
    public class Intersection
    {
        #region FRUSTUM<->POINT

#warning Trigger frustum points recalculation
        public static bool PointInsideFrustum(FrustumPointsPositions FrustumPointsWorldPositions, Vector3 worldPositionPoint)
        {
            return PointInsideFrustumComputationV2(worldPositionPoint, FrustumPointsWorldPositions);
        }

        private static bool PointInsideFrustumComputationV2(Vector3 worldPositionPoint, FrustumPointsPositions FrustumPointsWorldPositions)
        {
            return
                (Vector3.Dot(FrustumPointsWorldPositions.normal1, worldPositionPoint - FrustumPointsWorldPositions.FC1) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal2, worldPositionPoint - FrustumPointsWorldPositions.FC1) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal3, worldPositionPoint - FrustumPointsWorldPositions.FC2) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal4, worldPositionPoint - FrustumPointsWorldPositions.FC3) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal5, worldPositionPoint - FrustumPointsWorldPositions.FC4) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal6, worldPositionPoint - FrustumPointsWorldPositions.FC5) >= 0)
                ;
        }

        private static bool SegmentSphereIntersection(Vector3 segmentOrigin, Vector3 segmentDirection, float segmentDistance, Vector3 sphereCenterPoint, float sphereRadius)
        {
            // Segment sphere intersection https://en.wikipedia.org/wiki/Line%E2%80%93sphere_intersection

            float a = -1 * (Vector3.Dot(segmentDirection, segmentOrigin - sphereCenterPoint));
            float b = Mathf.Pow(-1 * a, 2) - ((segmentOrigin - sphereCenterPoint).sqrMagnitude - (sphereRadius * sphereRadius));

            bool intersect = false;

            if (b == 0)
            {
                float d = a;
                intersect = (d > 0 && d < segmentDistance);
            }

            if (b >= 0)
            {
                float d = a + Mathf.Sqrt(b);
                intersect = (d > 0 && d < segmentDistance);
                if (!intersect)
                {
                    d = a - Mathf.Sqrt(b);
                    intersect = (d > 0 && d < segmentDistance);
                }
            }

            return intersect;
        }

        #endregion

        /*
   *     C5----C6
   *    / |    /|
   *   C1----C2 |
   *   |  C8  | C7   
   *   | /    |/     C3->C7  Forward
   *   C4----C3     
   */

        public static void ExtractBoxColliderWorldPointsV2(BoxDefinition BoxDefinition, out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
        {
            ExtractBoxColliderLocalPoints(BoxDefinition.LocalCenter, BoxDefinition.LocalSize, out Vector3 lC1, out Vector3 lC2, out Vector3 lC3, out Vector3 lC4, out Vector3 lC5,
                out Vector3 lC6, out Vector3 lC7, out Vector3 lC8);

            var boxLocalToWorld = BoxDefinition.LocalToWorld;

            C1 = boxLocalToWorld.MultiplyPoint(lC1);
            C2 = boxLocalToWorld.MultiplyPoint(lC2);
            C3 = boxLocalToWorld.MultiplyPoint(lC3);
            C4 = boxLocalToWorld.MultiplyPoint(lC4);
            C5 = boxLocalToWorld.MultiplyPoint(lC5);
            C6 = boxLocalToWorld.MultiplyPoint(lC6);
            C7 = boxLocalToWorld.MultiplyPoint(lC7);
            C8 = boxLocalToWorld.MultiplyPoint(lC8);
        }

        public static void ExtractBoxColliderLocalPoints(Vector3 localCenter, Vector3 localSize, out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
        {
            Vector3 diagDirection = Vector3.zero;


            diagDirection = diagDirection.SetVector(-localSize.x, localSize.y, -localSize.z) * 0.5f;
            C1 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(localSize.x, localSize.y, -localSize.z) * 0.5f;
            C2 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(localSize.x, -localSize.y, -localSize.z) * 0.5f;
            C3 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(-localSize.x, -localSize.y, -localSize.z) * 0.5f;
            C4 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(-localSize.x, localSize.y, localSize.z) * 0.5f;
            C5 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(localSize.x, localSize.y, localSize.z) * 0.5f;
            C6 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(localSize.x, -localSize.y, localSize.z) * 0.5f;
            C7 = localCenter + diagDirection;

            diagDirection = diagDirection.SetVector(-localSize.x, -localSize.y, localSize.z) * 0.5f;
            C8 = localCenter + diagDirection;
        }

        public static FrustumPointsPositions ConvertBoxColliderToFrustumPoints(BoxDefinition BoxDefinition)
        {
            ExtractBoxColliderWorldPointsV2(BoxDefinition, out Vector3 FC1, out Vector3 FC2, out Vector3 FC3, out Vector3 FC4, out Vector3 FC5, out Vector3 FC6, out Vector3 FC7, out Vector3 FC8);
            return new FrustumPointsPositions(FC1, FC2, FC3, FC4, FC5, FC6, FC7, FC8);
        }
    }
}