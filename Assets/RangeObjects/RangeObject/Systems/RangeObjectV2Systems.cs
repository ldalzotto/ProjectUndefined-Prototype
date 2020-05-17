using System;
using CoreGame;
using InteractiveObjects;
using Obstacle;
using UnityEngine;

namespace RangeObjects
{
    public abstract class ARangeObjectSystem
    {
        protected RangeObjectV2 RangeObjectV2Ref;

        protected ARangeObjectSystem(RangeObjectV2 rangeObjectV2Ref)
        {
            RangeObjectV2Ref = rangeObjectV2Ref;
        }

        public virtual void Tick(float d)
        {
        }
    }

    #region Range Type

    public abstract class ARangeTypeDefinitionV2
    {
    }

    [SceneHandleDraw]
    [Serializable]
    public class SphereRangeTypeDefinition : ARangeTypeDefinitionV2
    {
        [WireCircle(R = 1f, G = 1f, B = 0f)] public float Radius;
    }

    [SceneHandleDraw]
    [Serializable]
    public class BoxRangeTypeDefinition : ARangeTypeDefinitionV2
    {
        [WireBox(R = 1f, G = 1f, B = 1f, CenterFieldName = nameof(BoxRangeTypeDefinition.Center), SizeFieldName = nameof(BoxRangeTypeDefinition.Size))]
        public Vector3 Center;

        public Vector3 Size;
    }

    [Serializable]
    public class FrustumRangeTypeDefinition : ARangeTypeDefinitionV2
    {
        public FrustumV2 FrustumV2;
    }

    [SceneHandleDraw]
    [Serializable]
    public class RoundedFrustumRangeTypeDefinition : ARangeTypeDefinitionV2
    {
        [WireRoundedFrustum(R = 1, G = 1, B = 0)]
        public FrustumV2 FrustumV2;
    }

    #endregion

    #region Range Obstacle Listener

    public class RangeObstacleListenerSystem : ARangeObjectSystem
    {
        private RangeObstaclePhysicsEventListener RangeObstaclePhysicsEventListener;

        public RangeObstacleListenerSystem(RangeObjectV2 rangeObjectV2Ref, RangeObjectV2PhysicsEventListenerComponent rangeObjectV2PhysicsEventListenerComponent) : base(rangeObjectV2Ref)
        {
            this.ObstacleListener = new ObstacleListenerSystem(new Func<TransformStruct>(() => rangeObjectV2Ref.GetTransform()));
            this.RangeObstaclePhysicsEventListener = new RangeObstaclePhysicsEventListener(this.ObstacleListener);
            rangeObjectV2PhysicsEventListenerComponent.AddPhysicsEventListener(this.RangeObstaclePhysicsEventListener);
        }

        public ObstacleListenerSystem ObstacleListener { get; private set; }

        public void OnDestroy()
        {
            this.RangeObstaclePhysicsEventListener.OnDestroy();
        }
    }

    public class RangeObstaclePhysicsEventListener : ARangeObjectV2PhysicsEventListener
    {
        private ObstacleListenerSystem AssociatedObstacleListener;
        private InteractiveObjectTagStruct SelectionGuard;

        public RangeObstaclePhysicsEventListener(ObstacleListenerSystem associatedObstacleListener)
        {
            AssociatedObstacleListener = associatedObstacleListener;
            this.SelectionGuard = new InteractiveObjectTagStruct(isObstacle: 1);
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return this.SelectionGuard.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.AssociatedObstacleListener.AddNearSquareObstacle((ObstacleInteractiveObject) PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            this.AssociatedObstacleListener.RemoveNearSquareObstacle((ObstacleInteractiveObject) PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public void OnDestroy()
        {
            this.AssociatedObstacleListener.OnObstacleListenerDestroyed();
        }
    }

    #endregion

    #region External Physics Only Listeners

    public class RangeExternalPhysicsOnlyListenersSystem : ARangeObjectSystem
    {
        private RangeObjectV2PhysicsEventListenerComponent _rangeObjectV2PhysicsEventListenerComponent;

        public RangeExternalPhysicsOnlyListenersSystem(RangeObjectV2 rangeObjectV2Ref, RangeObjectV2PhysicsEventListenerComponent rangeObjectV2PhysicsEventListenerComponent) : base(rangeObjectV2Ref)
        {
            this._rangeObjectV2PhysicsEventListenerComponent = rangeObjectV2PhysicsEventListenerComponent;
        }

        public void RegisterPhysicsEventListener(ARangeObjectV2PhysicsEventListener ARangeObjectV2PhysicsEventListener)
        {
            this._rangeObjectV2PhysicsEventListenerComponent.AddPhysicsEventListener(ARangeObjectV2PhysicsEventListener);
        }

        public void OnDestroy()
        {
            this._rangeObjectV2PhysicsEventListenerComponent.Destroy();
        }
    }

    #endregion

    #region Range Frustum Positioning

    public class FrustumRangeObjectPositioningSystem : ARangeObjectSystem
    {
        private RangeFrustumWorldPositioning RangeFrustumWorldPositioning;

        public FrustumRangeObjectPositioningSystem(FrustumV2 Frustum, RangeObjectV2 RoundedFrustumRangeObjectV2) : base(RoundedFrustumRangeObjectV2)
        {
            Frustum.CalculateFrustumWorldPositionyFace(out FrustumPointsPositions LocalFrustumPointPositions, new TransformStruct {WorldPosition = Vector3.zero, WorldRotationEuler = Vector3.zero, LossyScale = Vector3.one});
            this.RangeFrustumWorldPositioning = new RangeFrustumWorldPositioning
            {
                LocalFrustumPositions = LocalFrustumPointPositions
            };
        }

        public FrustumPointsPositions GetFrustumWorldPosition()
        {
            return this.RangeFrustumWorldPositioning.GetWorldFrustumPositions(this.RangeObjectV2Ref.RangeGameObjectV2.GetLocalToWorldMatrix());
        }
    }

    public struct RangeFrustumWorldPositioning
    {
        public FrustumPointsPositions LocalFrustumPositions;

        public FrustumPointsPositions GetWorldFrustumPositions(Matrix4x4 LocalToWorld)
        {
            return new FrustumPointsPositions(
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC1),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC2),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC3),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC4),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC5),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC6),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC7),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC8)
            );
        }
    }

    #endregion
}