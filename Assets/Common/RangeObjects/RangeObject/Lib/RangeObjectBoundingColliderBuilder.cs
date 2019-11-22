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
            Frustum.BuildLocalBoundingBox(out Vector3 localSize,out Vector3 localCenter);
            var boxCollider = RangeGameObject.RangeGameObject.AddComponent<BoxCollider>();
            boxCollider.center = localCenter;
            boxCollider.size = localSize;
            boxCollider.isTrigger = true;
            return boxCollider;
        }
    }
}