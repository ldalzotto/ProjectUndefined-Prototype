using System;
using GeometryIntersection;
using UnityEngine;

namespace RangeObjects
{
    public static class RangeObjectBoundingColliderBuilder
    {
        public static Collider BuildBoundingCollider(SphereRangeObjectInitialization SphereRangeObjectInitialization, RangeGameObjectV2 RangeGameObject)
        {
            var sphereCollider = RangeGameObject.RangeGameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = SphereRangeObjectInitialization.SphereRangeTypeDefinition.Radius;
            sphereCollider.isTrigger = true;
            return sphereCollider;
        }

        public static Collider BuildBoundingCollider(BoxRangeObjectInitialization BoxRangeObjectInitialization, RangeGameObjectV2 RangeGameObject)
        {
            var boxCollider = RangeGameObject.RangeGameObject.AddComponent<BoxCollider>();
            boxCollider.center = BoxRangeObjectInitialization.BoxRangeTypeDefinition.Center;
            boxCollider.size = BoxRangeObjectInitialization.BoxRangeTypeDefinition.Size;
            boxCollider.isTrigger = true;
            return boxCollider;
        }

        public static Collider BuildBoundingCollider(FrustumRangeObjectInitialization FrustumRangeObjectInitialization, RangeGameObjectV2 RangeGameObject)
        {
            return BuildBoundingCollider(FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2, RangeGameObject);
        }

        public static Collider BuildBoundingCollider(RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization, RangeGameObjectV2 RangeGameObject)
        {
            return BuildBoundingCollider(RoundedFrustumRangeObjectInitialization.RoundedFrustumRangeTypeDefinition.FrustumV2, RangeGameObject);
        }

        private static Collider BuildBoundingCollider(FrustumV2 Frustum, RangeGameObjectV2 RangeGameObject)
        {
            var boxCollider = RangeGameObject.RangeGameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 0, Frustum.F2.FaceOffsetFromCenter.z / 4f);
            boxCollider.size = new Vector3(Mathf.Max(Frustum.F1.Width, Frustum.F2.Width), Math.Max(Frustum.F1.Height, Frustum.F2.Height), Frustum.F2.FaceOffsetFromCenter.z / 2f);
            boxCollider.isTrigger = true;
            return boxCollider;
        }
    }
}