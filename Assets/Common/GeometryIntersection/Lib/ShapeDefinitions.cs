using System;
using CoreGame;
using UnityEngine;

namespace GeometryIntersection
{
    /*
     *     C5----C6
     *    / |    /|
     *   C1----C2 |
     *   |  C8  | C7   
     *   | /    |/     C3->C7  Forward
     *   C4----C3     
     */

    [Serializable]
    public struct FrustumV2
    {
        public Vector3 Center;
        public Vector3 DeltaRotation;
        public FrustumFaceV3 F1v3;
        public FrustumFaceV3 F2v3;
        public float FaceDistance;

        public float GetFrustumFaceRadius()
        {
            return this.F2v3.LocalC1.z;
        }

        private Vector3 LocalToWorld(TransformStruct FrustumTransform, Vector3 localPoint)
        {
            return Matrix4x4.TRS(Center + FrustumTransform.WorldPosition, Quaternion.Euler(FrustumTransform.WorldRotationEuler) * Quaternion.Euler(this.DeltaRotation),
                       FrustumTransform.LossyScale ).MultiplyPoint(localPoint);
        }

        /// <summary>
        /// Calculate Frustums based on point projectiion <paramref name="WorldStartAngleProjection"/>.
        /// Frustums calculation are done only if the <paramref name="WorldStartAngleProjection"/> is facing the <see cref="F1v3"/> face. <paramref name="IsFacing"/> flags
        /// indicates if calculation has been done or not.
        /// </summary>
        public void CalculateFrustumPointsWorldPosByProjection(out FrustumPointsPositions FrustumPointsPositions, out bool IsFacing, TransformStruct FrustumTransform, Vector3 WorldStartAngleProjection)
        {
         //   this.F1.CalculateLocalFacePoints(out Vector3 lC1, out Vector3 lC2, out Vector3 lC3, out Vector3 lC4);
            Vector3 C1 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC1);
            Vector3 C2 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC2);
            Vector3 C3 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC3);
            Vector3 C4 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC4);
            
           // Debug.Log(C1.ToString("F4"));

            Vector3 frontFaceNormal = Vector3.Cross(C2 - C1, C4 - C1).normalized;
            IsFacing = Vector3.Dot(frontFaceNormal, C1 - WorldStartAngleProjection) >= 0;

            //We abort calculation if not facing
            if (IsFacing)
            {
                Vector3 C5 = C1 + ((C1 - WorldStartAngleProjection) * this.FaceDistance);
                Vector3 C6 = C2 + ((C2 - WorldStartAngleProjection) * this.FaceDistance);
                Vector3 C7 = C3 + ((C3 - WorldStartAngleProjection) * this.FaceDistance);
                Vector3 C8 = C4 + ((C4 - WorldStartAngleProjection) * this.FaceDistance);
                FrustumPointsPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
            }
            else
            {
                FrustumPointsPositions = default;
            }
        }

        public void CalculateFrustumWorldPositionyFace(out FrustumPointsPositions FrustumPointsPositions, TransformStruct FrustumTransform)
        {
         
            Vector3 C1 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC1);
            Vector3 C2 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC2);
            Vector3 C3 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC3);
            Vector3 C4 = this.LocalToWorld(FrustumTransform, this.F1v3.LocalC4);
            Vector3 C5 = this.LocalToWorld(FrustumTransform, this.F2v3.LocalC1);
            Vector3 C6 = this.LocalToWorld(FrustumTransform, this.F2v3.LocalC2);
            Vector3 C7 = this.LocalToWorld(FrustumTransform, this.F2v3.LocalC3);
            Vector3 C8 = this.LocalToWorld(FrustumTransform, this.F2v3.LocalC4);
            
            FrustumPointsPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
        }
        
        /// <summary>
        /// The bounding box of a Face defined Frustum in frustum local space
        /// </summary>
        public void BuildLocalBoundingBox(out Vector3 LocalSize, out Vector3 LocalCenter)
        {
            Bounds bouds = new Bounds();
            
            bouds.Encapsulate(this.F1v3.LocalC1);
            bouds.Encapsulate(this.F1v3.LocalC2);
            bouds.Encapsulate(this.F1v3.LocalC3);
            bouds.Encapsulate(this.F1v3.LocalC4);
            bouds.Encapsulate(this.F2v3.LocalC1);
            bouds.Encapsulate(this.F2v3.LocalC2);
            bouds.Encapsulate(this.F2v3.LocalC3);
            bouds.Encapsulate(this.F2v3.LocalC4);
            
            LocalCenter = bouds.center;
            LocalSize = bouds.size;
        }
    }

    [Serializable]
    public struct FrustumFaceV2
    {
        public Vector3 FaceOffsetFromCenter;
        public float Height;
        public float Width;

        public void CalculateLocalFacePoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4)
        {
            C1 =  (new Vector3(-this.Width, this.Height, 0) + this.FaceOffsetFromCenter);
            C2 =  (new Vector3(this.Width, this.Height, 0) + this.FaceOffsetFromCenter);
            C3 =  (new Vector3(this.Width, -this.Height, 0) + this.FaceOffsetFromCenter);
            C4 =  (new Vector3(-this.Width, -this.Height, 0) + this.FaceOffsetFromCenter);
        }
    }

    [Serializable]
    public struct FrustumFaceV3
    {
        public Vector3 LocalC1;
        public Vector3 LocalC2;
        public Vector3 LocalC3;
        public Vector3 LocalC4;
    }

    [Serializable]
    public enum FrustumCalculationType
    {
        FACE,
        PROJECTION
    }

    [Serializable]
    public struct BoxDefinition
    {
        public Vector3 LocalCenter;
        public Vector3 LocalSize;
        public Matrix4x4 LocalToWorld;

        public BoxDefinition(BoxCollider BoxCollider)
        {
            this.LocalCenter = BoxCollider.center;
            this.LocalSize = BoxCollider.size;
            this.LocalToWorld = BoxCollider.transform.localToWorldMatrix;
        }
        
    }

}