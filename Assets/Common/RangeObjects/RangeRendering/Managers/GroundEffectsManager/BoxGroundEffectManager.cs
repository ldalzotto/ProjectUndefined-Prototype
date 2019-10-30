using CoreGame;
using UnityEngine;

namespace RangeObjects
{
    public class BoxGroundEffectManager : AbstractGroundEffectManager
    {
        private BoxRangeObjectRenderingDataProvider BoxRangeObjectRenderingDataProvider;

        public BoxGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData, BoxRangeObjectRenderingDataProvider BoxRangeObjectRenderingDataProvider) : base(rangeTypeInherentConfigurationData)
        {
            this.BoxRangeObjectRenderingDataProvider = BoxRangeObjectRenderingDataProvider;
        }

        public BoxRangeBufferData ToBoxBuffer()
        {
            var boxFrustum = Intersection.ConvertBoxColliderToFrustumPoints(BoxRangeObjectRenderingDataProvider.GetBoundingBoxDefinition());
            var boxRangeBufferData = new BoxRangeBufferData();

            boxRangeBufferData.FC1 = boxFrustum.FC1;
            boxRangeBufferData.FC2 = boxFrustum.FC2;
            boxRangeBufferData.FC3 = boxFrustum.FC3;
            boxRangeBufferData.FC4 = boxFrustum.FC4;
            boxRangeBufferData.FC5 = boxFrustum.FC5;

            boxRangeBufferData.normal1 = boxFrustum.normal1;
            boxRangeBufferData.normal2 = boxFrustum.normal2;
            boxRangeBufferData.normal3 = boxFrustum.normal3;
            boxRangeBufferData.normal4 = boxFrustum.normal4;
            boxRangeBufferData.normal5 = boxFrustum.normal5;
            boxRangeBufferData.normal6 = boxFrustum.normal6;

            boxRangeBufferData.BoundingBoxMax = BoxRangeObjectRenderingDataProvider.BoundingBoxCollider.bounds.max;
            boxRangeBufferData.BoundingBoxMin = BoxRangeObjectRenderingDataProvider.BoundingBoxCollider.bounds.min;

            if (rangeTypeInherentConfigurationData.RangeColorProvider != null)
                boxRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            else
                boxRangeBufferData.AuraColor = rangeTypeInherentConfigurationData.RangeBaseColor;

            return boxRangeBufferData;
        }
    }

    public struct BoxRangeBufferData
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

        public Vector4 AuraColor;

        public static int GetByteSize()
        {
            return (13 * 3 + 4) * sizeof(float);
        }
    }
}