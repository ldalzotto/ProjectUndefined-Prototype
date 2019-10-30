using System;
using UnityEngine;

namespace CoreGame
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
        public FrustumFaceV2 F1;
        public FrustumFaceV2 F2;
        public float FaceDistance;

        public float GetFrustumFaceRadius()
        {
            return this.F2.FaceOffsetFromCenter.z / 2f;
        }

        private Vector3 LocalToWorld(TransformStruct FrustumTransform, Vector3 localPoint)
        {
            return (FrustumTransform.WorldPosition + Quaternion.Euler(FrustumTransform.WorldRotationEuler) * ((Quaternion.Euler(this.DeltaRotation) * localPoint) + this.Center).Mul(FrustumTransform.LossyScale));
        }

        public void CalculateFrustumPointsWorldPosByProjection(out FrustumPointsPositions FrustumPointsPositions, out bool IsFacing, TransformStruct FrustumTransform, Vector3 WorldStartAngleProjection)
        {
            Vector3 C1 = this.LocalToWorld(FrustumTransform, (new Vector3(-this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C2 = this.LocalToWorld(FrustumTransform, (new Vector3(this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C3 = this.LocalToWorld(FrustumTransform, (new Vector3(this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C4 = this.LocalToWorld(FrustumTransform, (new Vector3(-this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));

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
            Vector3 C1 = this.LocalToWorld(FrustumTransform, (new Vector3(-this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C2 = this.LocalToWorld(FrustumTransform, (new Vector3(this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C3 = this.LocalToWorld(FrustumTransform, (new Vector3(this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C4 = this.LocalToWorld(FrustumTransform, (new Vector3(-this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C5 = this.LocalToWorld(FrustumTransform, (new Vector3(-this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C6 = this.LocalToWorld(FrustumTransform, (new Vector3(this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C7 = this.LocalToWorld(FrustumTransform, (new Vector3(this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C8 = this.LocalToWorld(FrustumTransform, (new Vector3(-this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
            FrustumPointsPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
        }
    }

    [Serializable]
    public struct FrustumFaceV2
    {
        public Vector3 FaceOffsetFromCenter;
        public float Height;
        public float Width;
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