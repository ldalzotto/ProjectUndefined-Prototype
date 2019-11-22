﻿using CoreGame;
using UnityEngine;

namespace GeometryIntersection
{
    public class Intersection
    {
        #region BOX->SPHERE

        public static bool BoxIntersectsOrEntirelyContainedInSphere(BoxDefinition BoxDefinition, Vector3 SphereWorldPosition, float SphereRadius)
        {
            return BoxEntirelyContainedInSphereV2(BoxDefinition, SphereWorldPosition, SphereRadius)
                   || BoxIntersectsSphereV2(BoxDefinition, SphereWorldPosition, SphereRadius);
        }

        private static bool BoxIntersectsSphereV2(BoxDefinition BoxDefinition, Vector3 SphereWorldPosition, float SphereRadius)
        {
            // -> Optimisations can be made in order to not trigger the full calcuation if objects are too far.
            // -> Also, order of faces can be sorted by distance check.
            ExtractBoxColliderWorldPointsV2(BoxDefinition,
                out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            //Face intersection
            return FaceIntersectSphere(BC6, BC7, BC5, BC8, SphereWorldPosition, SphereRadius)
                   || FaceIntersectSphere(BC2, BC3, BC1, BC4, SphereWorldPosition, SphereRadius)
                   || FaceIntersectSphere(BC6, BC7, BC2, BC3, SphereWorldPosition, SphereRadius)
                   || FaceIntersectSphere(BC5, BC8, BC1, BC4, SphereWorldPosition, SphereRadius)
                   || FaceIntersectSphere(BC6, BC2, BC5, BC1, SphereWorldPosition, SphereRadius)
                   || FaceIntersectSphere(BC7, BC3, BC8, BC4, SphereWorldPosition, SphereRadius);
        }


        private static bool BoxEntirelyContainedInSphereV2(BoxDefinition BoxDefinition, Vector3 SphereWorldPosition, float SphereRadius)
        {
            // -> Optimisations can be made in order to not trigger the full calcuation if objects are too far.
            // -> Also, order of faces can be sorted by distance check.
            ExtractBoxColliderWorldPointsV2(BoxDefinition, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);
            //Face contains
            return FaceEntirelyContainedInSphere(BC6, BC7, BC5, BC8, SphereWorldPosition, SphereRadius)
                   || FaceEntirelyContainedInSphere(BC2, BC3, BC1, BC4, SphereWorldPosition, SphereRadius)
                   || FaceEntirelyContainedInSphere(BC6, BC7, BC2, BC3, SphereWorldPosition, SphereRadius)
                   || FaceEntirelyContainedInSphere(BC5, BC8, BC1, BC4, SphereWorldPosition, SphereRadius)
                   || FaceEntirelyContainedInSphere(BC6, BC2, BC5, BC1, SphereWorldPosition, SphereRadius)
                   || FaceEntirelyContainedInSphere(BC7, BC3, BC8, BC4, SphereWorldPosition, SphereRadius);
        }

        #endregion

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
                && (Vector3.Dot(FrustumPointsWorldPositions.normal1, FrustumPointsWorldPositions.FC5 - FrustumPointsWorldPositions.FC1) > 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal2, worldPositionPoint - FrustumPointsWorldPositions.FC1) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal2, FrustumPointsWorldPositions.FC4 - FrustumPointsWorldPositions.FC1) > 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal3, worldPositionPoint - FrustumPointsWorldPositions.FC2) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal3, FrustumPointsWorldPositions.FC1 - FrustumPointsWorldPositions.FC2) > 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal4, worldPositionPoint - FrustumPointsWorldPositions.FC3) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal4, FrustumPointsWorldPositions.FC2 - FrustumPointsWorldPositions.FC3) > 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal5, worldPositionPoint - FrustumPointsWorldPositions.FC4) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal5, FrustumPointsWorldPositions.FC3 - FrustumPointsWorldPositions.FC4) > 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal6, worldPositionPoint - FrustumPointsWorldPositions.FC5) >= 0)
                && (Vector3.Dot(FrustumPointsWorldPositions.normal6, FrustumPointsWorldPositions.FC1 - FrustumPointsWorldPositions.FC5) > 0)
                ;
        }

        private static bool FaceIntersectSphere(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool planeIntersected = false;
            bool edgeIntersected = false;

            // (1) - We check if edges cross the sphere 
            edgeIntersected
                = SegmentSphereIntersection(C1, (C2 - C1).normalized, Vector3.Distance(C2, C1), SphereWorldPosition, SphereRadius)
                  || SegmentSphereIntersection(C1, (C3 - C1).normalized, Vector3.Distance(C3, C1), SphereWorldPosition, SphereRadius)
                  || SegmentSphereIntersection(C3, (C4 - C3).normalized, Vector3.Distance(C4, C3), SphereWorldPosition, SphereRadius)
                  || SegmentSphereIntersection(C2, (C4 - C2).normalized, Vector3.Distance(C4, C2), SphereWorldPosition, SphereRadius);

            // (2) - If edges doesn't cross the sphere, we try to find the intersection from sphere to cube face plane //http://www.ambrsoft.com/TrigoCalc/Sphere/SpherePlaneIntersection_.htm
            // Intersection is valid only if intercention circle center is contained insibe cube face https://math.stackexchange.com/a/190373
            if (!edgeIntersected)
            {
                Vector3 normal = Vector3.Cross(C2 - C1, C3 - C1);
                float a = normal.x;
                float b = normal.y;
                float c = normal.z;
                float d = -1 * (a * C1.x + b * C1.y + c * C1.z);

                float nom = (a * SphereWorldPosition.x + b * SphereWorldPosition.y + c * SphereWorldPosition.z + d);
                float denom = (a * a) + (b * b) + (c * c);

                Vector3 planeIntersectionPoint = new Vector3(
                    SphereWorldPosition.x - (a * nom / denom),
                    SphereWorldPosition.y - (b * nom / denom),
                    SphereWorldPosition.z - (c * nom / denom)
                );

                if (Vector3.Distance(planeIntersectionPoint, SphereWorldPosition) <= SphereRadius)
                {
                    planeIntersected = Vector3.Dot(C2 - C1, C2 - C1) > Vector3.Dot(planeIntersectionPoint - C1, C2 - C1) && Vector3.Dot(planeIntersectionPoint - C1, C2 - C1) > 0
                                                                                                                         && Vector3.Dot(C3 - C1, C3 - C1) > Vector3.Dot(planeIntersectionPoint - C1, C3 - C1) && Vector3.Dot(planeIntersectionPoint - C1, C3 - C1) > 0;
                }
            }

            return (planeIntersected || edgeIntersected);
        }

        private static bool FaceEntirelyContainedInSphere(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool edgeContained = false;

            // (1) - We check if edges are fully contained inside
            edgeContained
                = SegmentEntirelyContainedInSphere(C1, (C2 - C1).normalized, Vector3.Distance(C2, C1), SphereWorldPosition, SphereRadius)
                  || SegmentEntirelyContainedInSphere(C1, (C3 - C1).normalized, Vector3.Distance(C3, C1), SphereWorldPosition, SphereRadius)
                  || SegmentEntirelyContainedInSphere(C3, (C4 - C3).normalized, Vector3.Distance(C4, C3), SphereWorldPosition, SphereRadius)
                  || SegmentEntirelyContainedInSphere(C2, (C4 - C2).normalized, Vector3.Distance(C4, C2), SphereWorldPosition, SphereRadius);

            return edgeContained;
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

        private static bool SegmentEntirelyContainedInSphere(Vector3 segmentOrigin, Vector3 segmentDirection, float segmentDistance, Vector3 sphereCenterPoint, float sphereRadius)
        {
            return Vector3.Distance(segmentOrigin, sphereCenterPoint) <= sphereRadius && Vector3.Distance(segmentOrigin + (segmentDirection * segmentDistance), sphereCenterPoint) <= sphereRadius;
        }

        #endregion


        #region FRUSTUM<->BOX

        public static bool BoxEntirelyContainedInFrustum(FrustumPointsPositions frustumPoints, BoxDefinition BoxDefinition)
        {
            ExtractBoxColliderWorldPointsV2(BoxDefinition, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);
            return (PointInsideFrustumComputationV2(BC1, frustumPoints)
                    && PointInsideFrustumComputationV2(BC2, frustumPoints)
                    && PointInsideFrustumComputationV2(BC3, frustumPoints)
                    && PointInsideFrustumComputationV2(BC4, frustumPoints)
                    && PointInsideFrustumComputationV2(BC5, frustumPoints)
                    && PointInsideFrustumComputationV2(BC6, frustumPoints)
                    && PointInsideFrustumComputationV2(BC7, frustumPoints)
                    && PointInsideFrustumComputationV2(BC8, frustumPoints));
        }

        public static bool FrustumBoxIntersection(FrustumPointsPositions frustumPoints, BoxDefinition BoxDefinition)
        {
            ExtractBoxColliderWorldPointsV2(BoxDefinition, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            if (!LineFrustumIntersection(BC1, BC2, frustumPoints) && !LineFrustumIntersection(BC2, BC4, frustumPoints) && !LineFrustumIntersection(BC4, BC3, frustumPoints) && !LineFrustumIntersection(BC3, BC1, frustumPoints)
                && !LineFrustumIntersection(BC1, BC5, frustumPoints) && !LineFrustumIntersection(BC2, BC6, frustumPoints) && !LineFrustumIntersection(BC3, BC7, frustumPoints) && !LineFrustumIntersection(BC4, BC8, frustumPoints)
                && !LineFrustumIntersection(BC7, BC8, frustumPoints) && !LineFrustumIntersection(BC8, BC6, frustumPoints) && !LineFrustumIntersection(BC6, BC5, frustumPoints) && !LineFrustumIntersection(BC5, BC7, frustumPoints))
            {
                return false;
            }

            return true;
        }

        #endregion

        public static bool LineFrustumIntersection(Vector3 lineOrigin, Vector3 lineEnd, FrustumPointsPositions frustumPoints)
        {
            return SegmentAccuratePlaneIntersectionV2(lineOrigin, lineEnd, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4)
                   || SegmentAccuratePlaneIntersectionV2(lineOrigin, lineEnd, frustumPoints.FC1, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC2)
                   || SegmentAccuratePlaneIntersectionV2(lineOrigin, lineEnd, frustumPoints.FC2, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC3)
                   || SegmentAccuratePlaneIntersectionV2(lineOrigin, lineEnd, frustumPoints.FC3, frustumPoints.FC7, frustumPoints.FC8, frustumPoints.FC4)
                   || SegmentAccuratePlaneIntersectionV2(lineOrigin, lineEnd, frustumPoints.FC4, frustumPoints.FC8, frustumPoints.FC5, frustumPoints.FC1)
                   || SegmentAccuratePlaneIntersectionV2(lineOrigin, lineEnd, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8);
        }

        //http://geomalgorithms.com/a05-_intersect-1.html
        /*
         * 
             // intersect3D_SegmentPlane(): find the 3D intersection of a segment and a plane
    //    Input:  S = a segment, and Pn = a plane = {Point V0;  Vector n;}
    //    Output: *I0 = the intersect point (when it exists)
    //    Return: 0 = disjoint (no intersection)
    //            1 =  intersection in the unique point *I0
    //            2 = the  segment lies in the plane
    int
    intersect3D_SegmentPlane( Segment S, Plane Pn, Point* I )
    {
        Vector    u = S.P1 - S.P0;
        Vector    w = S.P0 - Pn.V0;

        float     D = dot(Pn.n, u);
        float     N = -dot(Pn.n, w);

        if (fabs(D) < SMALL_NUM) {           // segment is parallel to plane
            if (N == 0)                      // segment lies in plane
                return 2;
            else
                return 0;                    // no intersection
        }
        // they are not parallel
        // compute intersect param
        float sI = N / D;
        if (sI < 0 || sI > 1)
            return 0;                        // no intersection

        *I = S.P0 + sI * u;                  // compute segment intersect point
        return 1;
    }
    //===================================================================
        */
        public static bool SegmentAccuratePlaneIntersectionV2(Vector3 segmentStart, Vector3 segmentEnd, Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
        {
            Vector3 u = segmentEnd - segmentStart;
            Vector3 w = segmentStart - C1;

            Vector3 normal = Vector3.Cross(C2 - C1, C3 - C1);
            float D = Vector3.Dot(normal, u);
            float N = -Vector3.Dot(normal, w);

            //if segment and plane are //

            if (Mathf.Abs(D) == 0)
            {
                return false;
            }

            //If lines extremities are in the inside normal side

            float sI = N / D;
            if (sI < 0 || sI > 1)
            {
                return false;
            }

            Vector3 I = segmentStart + (sI * u);

            //If the intersection point is outside of the plane
            //We project intersection point to normal
            Vector3 center = (C1 + C2 + C3 + C4) / 4f;

            Vector3 normal12 = (center - C1) - (((C2 - C1).normalized) * ((center - C1).magnitude * Mathf.Cos(Vector3.Angle(C2 - C1, center - C1) * Mathf.Deg2Rad)));
            Vector3 normal23 = (center - C2) - (((C3 - C2).normalized) * ((center - C2).magnitude * Mathf.Cos(Vector3.Angle(C3 - C2, center - C2) * Mathf.Deg2Rad)));
            Vector3 normal34 = (center - C3) - (((C4 - C3).normalized) * ((center - C3).magnitude * Mathf.Cos(Vector3.Angle(C4 - C3, center - C3) * Mathf.Deg2Rad)));
            Vector3 normal41 = (center - C4) - (((C1 - C4).normalized) * ((center - C4).magnitude * Mathf.Cos(Vector3.Angle(C1 - C4, center - C4) * Mathf.Deg2Rad)));


            if (Vector3.Dot(normal12, I - C1) >= 0 && Vector3.Dot(normal23, I - C2) >= 0 && Vector3.Dot(normal34, I - C3) >= 0 && Vector3.Dot(normal41, I - C4) >= 0)
            {
                return true;
            }

            return false;
        }

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