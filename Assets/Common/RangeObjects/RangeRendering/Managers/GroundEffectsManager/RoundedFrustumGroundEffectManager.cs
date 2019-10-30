using UnityEngine;

namespace RangeObjects
{
    public class RoundedFrustumGroundEffectManager : AbstractGroundEffectManager
    {
        private FrustumRangeObjectRenderingDataProvider RoundedFrustumRangeObjectRenderingDataProvider;

        public RoundedFrustumGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData, FrustumRangeObjectRenderingDataProvider RoundedFrustumRangeObjectRenderingDataProvider) : base(rangeTypeInherentConfigurationData)
        {
            this.RoundedFrustumRangeObjectRenderingDataProvider = RoundedFrustumRangeObjectRenderingDataProvider;
        }

        public RoundedFrustumRangeBufferData ToFrustumBuffer()
        {
            var RoundedFrustumRangeBufferData = new RoundedFrustumRangeBufferData();

            var frustumPointsWorldPositions = RoundedFrustumRangeObjectRenderingDataProvider.GetFrustumWorldPosition();
            RoundedFrustumRangeBufferData.FC1 = frustumPointsWorldPositions.FC1;
            RoundedFrustumRangeBufferData.FC2 = frustumPointsWorldPositions.FC2;
            RoundedFrustumRangeBufferData.FC3 = frustumPointsWorldPositions.FC3;
            RoundedFrustumRangeBufferData.FC4 = frustumPointsWorldPositions.FC4;
            RoundedFrustumRangeBufferData.FC5 = frustumPointsWorldPositions.FC5;
            RoundedFrustumRangeBufferData.normal1 = frustumPointsWorldPositions.normal1;
            RoundedFrustumRangeBufferData.normal2 = frustumPointsWorldPositions.normal2;
            RoundedFrustumRangeBufferData.normal3 = frustumPointsWorldPositions.normal3;
            RoundedFrustumRangeBufferData.normal4 = frustumPointsWorldPositions.normal4;
            RoundedFrustumRangeBufferData.normal5 = frustumPointsWorldPositions.normal5;
            RoundedFrustumRangeBufferData.normal6 = frustumPointsWorldPositions.normal6;


            RoundedFrustumRangeBufferData.BoundingBoxMax = RoundedFrustumRangeObjectRenderingDataProvider.BoundingCollider.bounds.max;
            RoundedFrustumRangeBufferData.BoundingBoxMin = RoundedFrustumRangeObjectRenderingDataProvider.BoundingCollider.bounds.min;

            RoundedFrustumRangeBufferData.RangeRadius = RoundedFrustumRangeObjectRenderingDataProvider.Frustum.GetFrustumFaceRadius();
            RoundedFrustumRangeBufferData.CenterWorldPosition = RoundedFrustumRangeObjectRenderingDataProvider.BoundingCollider.transform.position;

            if (rangeTypeInherentConfigurationData.RangeColorProvider != null)
                RoundedFrustumRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            else
                RoundedFrustumRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeBaseColor;

            if (RoundedFrustumRangeObjectRenderingDataProvider.IsTakingObstacleIntoConsideration())
                RoundedFrustumRangeBufferData.OccludedByFrustums = 1;
            else
                RoundedFrustumRangeBufferData.OccludedByFrustums = 0;

            return RoundedFrustumRangeBufferData;
        }
    }

    public struct RoundedFrustumRangeBufferData
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;
        public Vector3 FC5;

        public Vector3 normal1;
        public Vector3 normal2;
        public Vector3 normal3;
        public Vector3 normal4;
        public Vector3 normal5;
        public Vector3 normal6;

        public Vector3 BoundingBoxMax;
        public Vector3 BoundingBoxMin;

        public float RangeRadius;
        public Vector3 CenterWorldPosition;

        public int OccludedByFrustums;
        public Vector4 AuraColor;

        public static int GetByteSize()
        {
            return (3 * 5 + 3 * 6 + 3 * 2 + 4 + 1 + 3) * sizeof(float) + 1 * sizeof(int);
        }
    };
}