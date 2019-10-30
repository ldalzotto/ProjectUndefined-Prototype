using UnityEngine;

namespace RangeObjects
{
    public class SphereGroundEffectManager : AbstractGroundEffectManager
    {
        private SphereRangeObjectRenderingDataProvider SphereRangeObjectRenderingDataProvider;

        public SphereGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData,
            SphereRangeObjectRenderingDataProvider SphereRangeObjectRenderingDataProvider) : base(rangeTypeInherentConfigurationData)
        {
            this.SphereRangeObjectRenderingDataProvider = SphereRangeObjectRenderingDataProvider;
        }

        public CircleRangeBufferData ToSphereBuffer()
        {
            var CircleRangeBufferData = new CircleRangeBufferData();
            CircleRangeBufferData.CenterWorldPosition = SphereRangeObjectRenderingDataProvider.GetWorldPositionCenter();

            CircleRangeBufferData.Radius = SphereRangeObjectRenderingDataProvider.GetRadius();
            if (rangeTypeInherentConfigurationData.RangeColorProvider != null)
                CircleRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            else
                CircleRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeBaseColor;

            if (rangeObjectRenderingDataProvider.IsTakingObstacleIntoConsideration())
                CircleRangeBufferData.OccludedByFrustums = 1;
            else
                CircleRangeBufferData.OccludedByFrustums = 0;

            return CircleRangeBufferData;
        }
    }

    public struct CircleRangeBufferData
    {
        public Vector3 CenterWorldPosition;
        public float Radius;
        public Vector4 AuraColor;
        public int OccludedByFrustums;

        public static int GetByteSize()
        {
            return (3 + 1 + 4) * sizeof(float) + 1 * sizeof(int);
        }
    }
}