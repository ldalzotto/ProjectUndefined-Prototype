using CoreGame;
using Obstacle;
using UnityEngine;

namespace RangeObjects
{
    public abstract class ARangeObjectRenderingDataProvider
    {
        protected RangeObjectV2 RangeObjectV2Ref;

        protected ARangeObjectRenderingDataProvider(RangeObjectV2 rangeObjectV2Ref, RangeTypeID RangeTypeID)
        {
            RangeObjectV2Ref = rangeObjectV2Ref;
            this.RangeTypeID = RangeTypeID;
            this.BoundingCollider = rangeObjectV2Ref.RangeGameObjectV2.BoundingCollider;
            this.ObstacleListener = rangeObjectV2Ref.GetObstacleListener();
        }

        public RangeTypeID RangeTypeID { get; private set; }
        public Collider BoundingCollider { get; private set; }
        public ObstacleListenerSystem ObstacleListener { get; private set; }

        public bool IsTakingObstacleIntoConsideration()
        {
            return this.RangeObjectV2Ref.RangeObstacleListenerSystem != null;
        }
    }

    public class BoxRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {
        public BoxRangeObjectRenderingDataProvider(BoxRangeObjectV2 BoxRangeObjectV2, RangeTypeID RangeTypeID) : base(BoxRangeObjectV2, RangeTypeID)
        {
            this.BoundingBoxCollider = (BoxCollider) ((BoxRangeObjectV2) this.RangeObjectV2Ref).RangeGameObjectV2.BoundingCollider;
        }

        public BoxCollider BoundingBoxCollider { get; private set; }

        public BoxDefinition GetBoundingBoxDefinition()
        {
            return ((BoxRangeObjectV2) this.RangeObjectV2Ref).GetBoxBoundingColliderDefinition();
        }
    }

    public class SphereRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {
        public SphereRangeObjectRenderingDataProvider(SphereRangeObjectV2 SphereRangeObjectV2, RangeTypeID RangeTypeID) : base(SphereRangeObjectV2, RangeTypeID)
        {
            this.BoundingSphereCollider = SphereRangeObjectV2.SphereBoundingCollider;
        }

        public SphereCollider BoundingSphereCollider { get; private set; }

        public float GetRadius()
        {
            return this.BoundingSphereCollider.radius;
        }

        public Vector3 GetWorldPositionCenter()
        {
            return this.BoundingSphereCollider.transform.position;
        }
    }

    public class FrustumRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {
        private FrustumRangeObjectPositioningSystem FrustumRangeObjectPositioningSystem;

        public FrustumRangeObjectRenderingDataProvider(RoundedFrustumRangeObjectV2 RoundedFrustumRangeObjectV2, RangeTypeID RangeTypeID) : base(RoundedFrustumRangeObjectV2, RangeTypeID)
        {
            this.FrustumRangeObjectPositioningSystem = RoundedFrustumRangeObjectV2.FrustumRangeObjectPositioningSystem;
            this.Frustum = RoundedFrustumRangeObjectV2.GetFrustum();
        }

        public FrustumV2 Frustum { get; private set; }

        public FrustumPointsPositions GetFrustumWorldPosition()
        {
            return this.FrustumRangeObjectPositioningSystem.GetFrustumWorldPosition();
        }
    }
}