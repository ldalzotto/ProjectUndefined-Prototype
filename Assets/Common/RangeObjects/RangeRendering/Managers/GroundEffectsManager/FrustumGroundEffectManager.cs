using Obstacle;
using UnityEngine;

namespace RangeObjects
{
    public class FrustumGroundEffectManager : AbstractGroundEffectManager
    {
        private FrustumRangeObjectRenderingDataProvider FrustumRangeObjectRenderingDataprovider;

        #region External Dependencies

        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2;

        #endregion

        public FrustumGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData, FrustumRangeObjectRenderingDataProvider FrustumRangeObjectRenderingDataprovider) : base(rangeTypeInherentConfigurationData)
        {
            this.FrustumRangeObjectRenderingDataprovider = FrustumRangeObjectRenderingDataprovider;
            ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        }

        public FrustumRangeBufferData ToFrustumBuffer()
        {
            var FrustumRangeBufferData = new FrustumRangeBufferData();
            var frustumPointsLocalPositions = FrustumRangeObjectRenderingDataprovider.GetFrustumWorldPosition();

            FrustumRangeBufferData.FC1 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC1);
            FrustumRangeBufferData.FC2 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC2);
            FrustumRangeBufferData.FC3 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC3);
            FrustumRangeBufferData.FC4 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC4);
            FrustumRangeBufferData.FC5 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC5);
            FrustumRangeBufferData.FC6 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC6);
            FrustumRangeBufferData.FC7 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC7);
            FrustumRangeBufferData.FC8 = FrustumRangeObjectRenderingDataprovider.BoundingCollider.transform.TransformPoint(frustumPointsLocalPositions.FC8);

            if (rangeTypeInherentConfigurationData.RangeColorProvider != null)
                FrustumRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            else
                FrustumRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeBaseColor;

            if (rangeObjectRenderingDataProvider.IsTakingObstacleIntoConsideration())
                FrustumRangeBufferData.OccludedByFrustums = 1;
            else
                FrustumRangeBufferData.OccludedByFrustums = 0;

            return FrustumRangeBufferData;
        }
    }

    public struct FrustumRangeBufferData
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;
        public Vector3 FC5;
        public Vector3 FC6;
        public Vector3 FC7;
        public Vector3 FC8;

        public int OccludedByFrustums;
        public Vector4 AuraColor;

        public static int GetByteSize()
        {
            return (3 * 8 + 4) * sizeof(float) + 1 * sizeof(int);
        }
    };
}