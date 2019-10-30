using CoreGame;
using InteractiveObjects;
using Obstacle;
using UnityEngine;
using UnityEngine.Profiling;

namespace RangeObjects
{
    public abstract class RangeObjectV2
    {
        #region External Dependencies

        private RangeEventsManager RangeEventsManager = RangeEventsManager.Get();

        #endregion

        private RangeExternalPhysicsOnlyListenersSystem RangeExternalPhysicsOnlyListenersSystem;

        public RangeType RangeType { get; protected set; }
        public RangeGameObjectV2 RangeGameObjectV2 { get; private set; }

        public RangeObjectInitialization RangeObjectInitialization { get; private set; }

        public RangeObstacleListenerSystem RangeObstacleListenerSystem { get; private set; }
        public RangeIntersectionV2System RangeIntersectionV2System { get; private set; }

        protected void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
        {
            this.RangeGameObjectV2 = RangeGameObjectV2;
            this.RangeObjectInitialization = RangeObjectInitialization;

            this.RangeIntersectionV2System = new RangeIntersectionV2System(this);
            this.RangeExternalPhysicsOnlyListenersSystem = new RangeExternalPhysicsOnlyListenersSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListenerComponent);
            if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
            {
                this.RangeObstacleListenerSystem = new RangeObstacleListenerSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListenerComponent);
            }

            this.RangeEventsManager.OnRangeObjectCreated(this);
        }

        public virtual void Tick(float d)
        {
            Profiler.BeginSample("RangeObjectV2 : Tick");
            this.RangeIntersectionV2System.Tick(d);
            Profiler.EndSample();
        }

        public void OnDestroy()
        {
            //we call listeners callbacks
            this.RangeIntersectionV2System.OnDestroy();
            if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
            {
                this.RangeObstacleListenerSystem.OnDestroy();
            }

            this.RangeExternalPhysicsOnlyListenersSystem.OnDestroy();
            //To trigger itnersection events
            this.RangeIntersectionV2System.Tick(0f);
            this.RangeEventsManager.OnRangeObjectDestroyed(this);
        }

        public void ReceiveEvent(SetWorldPositionEvent SetWorldPositionEvent)
        {
            this.RangeGameObjectV2.ReceiveEvent(SetWorldPositionEvent);
        }

        public void RegisterIntersectionEventListener(ARangeIntersectionV2Listener ARangeIntersectionV2Listener)
        {
            this.RangeIntersectionV2System.RegisterIntersectionEventListener(ARangeIntersectionV2Listener, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListenerComponent);
        }

        public void RegisterPhysicsEventListener(ARangeObjectV2PhysicsEventListener ARangeObjectV2PhysicsEventListener)
        {
            this.RangeExternalPhysicsOnlyListenersSystem.RegisterPhysicsEventListener(ARangeObjectV2PhysicsEventListener);
        }

        public ObstacleListenerSystem GetObstacleListener()
        {
            return this.RangeObstacleListenerSystem != null ? this.RangeObstacleListenerSystem.ObstacleListener : null;
        }

        public TransformStruct GetTransform()
        {
            return this.RangeGameObjectV2.GetTransform();
        }
    }

    public class SphereRangeObjectV2 : RangeObjectV2
    {
        private SphereRangeObjectInitialization SphereRangeObjectInitialization;

        public SphereRangeObjectV2(GameObject AssociatedGameObject, SphereRangeObjectInitialization SphereRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.SPHERE;
            this.SphereRangeObjectInitialization = SphereRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.SphereRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            this.SphereBoundingCollider = (SphereCollider) RangeGameObjectV2.BoundingCollider;
            base.Init(RangeGameObjectV2, SphereRangeObjectInitialization);
        }

        public SphereCollider SphereBoundingCollider { get; private set; }
    }

    public class BoxRangeObjectV2 : RangeObjectV2
    {
        private BoxRangeObjectInitialization BoxRangeObjectInitialization;

        public BoxRangeObjectV2(GameObject AssociatedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.BOX;
            this.BoxRangeObjectInitialization = BoxRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.BoxRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            base.Init(RangeGameObjectV2, BoxRangeObjectInitialization);
        }

        public BoxDefinition GetBoxBoundingColliderDefinition()
        {
            var BoxCollider = (BoxCollider) RangeGameObjectV2.BoundingCollider;
            return new BoxDefinition(BoxCollider);
        }
    }

    public class FrustumRangeObjectV2 : RangeObjectV2
    {
        private FrustumRangeObjectInitialization FrustumRangeObjectInitialization;

        private FrustumRangeObjectPositioningSystem FrustumRangeObjectPositioningSystem;

        public FrustumRangeObjectV2(GameObject AssociatedGameObject, FrustumRangeObjectInitialization FrustumRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.FRUSTUM;

            this.FrustumRangeObjectInitialization = FrustumRangeObjectInitialization;
            this.FrustumRangeObjectPositioningSystem = new FrustumRangeObjectPositioningSystem(this.GetFrustum(), this);
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.FrustumRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            base.Init(RangeGameObjectV2, FrustumRangeObjectInitialization);
        }

        public FrustumV2 GetFrustum()
        {
            return this.FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2;
        }

        public FrustumPointsPositions GetFrustumWorldPositions()
        {
            return this.FrustumRangeObjectPositioningSystem.GetFrustumWorldPosition();
        }
    }

    [SceneHandleDraw]
    public class RoundedFrustumRangeObjectV2 : RangeObjectV2
    {
        [DrawNested] private RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization;

        public RoundedFrustumRangeObjectV2(GameObject AssociatedGameObject, RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            this.RangeType = RangeType.ROUNDED_FRUSTUM;

            this.RoundedFrustumRangeObjectInitialization = RoundedFrustumRangeObjectInitialization;
            this.FrustumRangeObjectPositioningSystem = new FrustumRangeObjectPositioningSystem(this.GetFrustum(), this);

            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.RoundedFrustumRangeObjectInitialization, this, AssociatedInteractiveObject, objectName);
            base.Init(RangeGameObjectV2, RoundedFrustumRangeObjectInitialization);
        }

        public FrustumRangeObjectPositioningSystem FrustumRangeObjectPositioningSystem { get; private set; }

        public FrustumV2 GetFrustum()
        {
            return this.RoundedFrustumRangeObjectInitialization.RoundedFrustumRangeTypeDefinition.FrustumV2;
        }

        public FrustumPointsPositions GetFrustumWorldPositions()
        {
            return this.FrustumRangeObjectPositioningSystem.GetFrustumWorldPosition();
        }
    }

    public struct SetWorldPositionEvent
    {
        public Vector3 WorldPosition;
    }

    public enum RangeType
    {
        SPHERE,
        BOX,
        FRUSTUM,
        ROUNDED_FRUSTUM
    }

    public static class RangeObjectV2Builder
    {
        public static RangeObjectV2 Build(GameObject AssociatedGameObject, RangeObjectInitialization RangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject, string objectName = "")
        {
            switch (RangeObjectInitialization)
            {
                case SphereRangeObjectInitialization SphereRangeObjectInitialization:
                    return new SphereRangeObjectV2(AssociatedGameObject, SphereRangeObjectInitialization, AssociatedInteractiveObject, objectName);
                case BoxRangeObjectInitialization BoxRangeObjectInitialization:
                    return new BoxRangeObjectV2(AssociatedGameObject, BoxRangeObjectInitialization, AssociatedInteractiveObject, objectName);
                case FrustumRangeObjectInitialization FrustumRangeObjectInitialization:
                    return new FrustumRangeObjectV2(AssociatedGameObject, FrustumRangeObjectInitialization, AssociatedInteractiveObject, objectName);
                case RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization:
                    return new RoundedFrustumRangeObjectV2(AssociatedGameObject, RoundedFrustumRangeObjectInitialization, AssociatedInteractiveObject, objectName);
            }

            return null;
        }
    }
}