using System;
using GeometryIntersection;
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

        /*
            Listens to physics engine trigger.
         */
        private RangeExternalPhysicsOnlyListenersSystem RangeExternalPhysicsOnlyListenersSystem;

        public RangeType RangeType { get; protected set; }
        public RangeGameObjectV2 RangeGameObjectV2 { get; private set; }

        public RangeObjectInitialization RangeObjectInitialization { get; private set; }

        public RangeObstacleListenerSystem RangeObstacleListenerSystem { get; private set; }
        public RangeIntersectionV2System RangeIntersectionV2System { get; private set; }


        /// <summary>
        /// /!\ <see cref="RangeObjectV2"/> destroy event hook.
        /// This is the **ONLY** way to detect destroy of range objects.
        /// This is to allow singular systems to have their own <see cref="RangeObjectV2"/> reference cleanup logic at RangeObject granularity. 
        /// </summary>
        private event Action<RangeObjectV2> OnRangeObjectDestroyedEvent;

        protected void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
        {
            this.RangeGameObjectV2 = RangeGameObjectV2;
            this.RangeObjectInitialization = RangeObjectInitialization;

            this.RangeIntersectionV2System = new RangeIntersectionV2System(this);
            this.RangeExternalPhysicsOnlyListenersSystem = new RangeExternalPhysicsOnlyListenersSystem(this, this.RangeGameObjectV2.InteractiveInteractiveObjectPhysicsListener);
            if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
            {
                this.RangeObstacleListenerSystem = new RangeObstacleListenerSystem(this, this.RangeGameObjectV2.InteractiveInteractiveObjectPhysicsListener);
            }

            this.RangeEventsManager.OnRangeObjectCreated(this);
        }

        public virtual void Tick(float d)
        {
            Profiler.BeginSample("RangeObjectV2 : Tick");
            this.RangeIntersectionV2System.Tick(d);
            Profiler.EndSample();
        }

        public virtual void OnDestroy()
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
            this.OnRangeObjectDestroyedEvent?.Invoke(this);
            
            GameObject.Destroy(this.RangeGameObjectV2.RangeGameObject);
        }

        public void ReceiveEvent(SetWorldPositionEvent SetWorldPositionEvent)
        {
            this.RangeGameObjectV2.ReceiveEvent(SetWorldPositionEvent);
        }

        public void RegisterIntersectionEventListener(AInteractiveIntersectionListener aInteractiveIntersectionListener)
        {
            this.RangeIntersectionV2System.RegisterIntersectionEventListener(aInteractiveIntersectionListener, this.RangeGameObjectV2.InteractiveInteractiveObjectPhysicsListener);
        }

        public void RegisterPhysicsEventListener(AInteractiveObjectPhysicsEventListener aInteractiveObjectPhysicsEventListener)
        {
            this.RangeExternalPhysicsOnlyListenersSystem.RegisterPhysicsEventListener(aInteractiveObjectPhysicsEventListener);
        }

        public void RegisterOnRangeObjectDestroyedEventListener(Action<RangeObjectV2> action)
        {
            this.OnRangeObjectDestroyedEvent += action;
        }

        public void UnRegisterOnRangeObjectDestroyedEventListener(Action<RangeObjectV2> action)
        {
            this.OnRangeObjectDestroyedEvent -= action;
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