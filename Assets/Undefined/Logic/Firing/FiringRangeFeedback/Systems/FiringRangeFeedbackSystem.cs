using System.Collections.Generic;
using CoreGame;
using FiredProjectile;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using RangeObjects;
using Targetting;
using UnityEngine;

namespace Firing
{
    /// <summary>
    /// Spawns a <see cref="_firingRangeFeedbackRangeObject"/> that will show where the Player is aiming at.
    /// This is visualized by the model of <see cref="_firingRangeFeedbackRangeObject"/>.
    /// </summary>
    public struct FiringRangeFeedbackSystem
    {
        private CoreInteractiveObject PlayerInteractiveObject;
        private TargettableInteractiveObjectSelectionManager TargettableInteractiveObjectSelectionManager;

        private FiringRangeFeedbackRangeObject _firingRangeFeedbackRangeObject;

        public FiringRangeFeedbackSystem(CoreInteractiveObject playerInteractiveObject, TargettableInteractiveObjectSelectionManager targettableInteractiveObjectSelectionManager)
        {
            PlayerInteractiveObject = playerInteractiveObject;
            TargettableInteractiveObjectSelectionManager = targettableInteractiveObjectSelectionManager;
            this._firingRangeFeedbackRangeObject = new FiringRangeFeedbackRangeObject(
                InteractiveGameObjectFactory.Build(new GameObject("FiringRangeFeedbackRangeObject")),
                FiringRangeFeedbackConfigurationGameObject.Get().firingRangeVisualFeedbackConfiguration.FiredProjectileFeedbackPrefab,
                playerInteractiveObject);
        }

        public void Tick(float d)
        {
            var targetSegment = CalculateFiringTargetPosition();

            this._firingRangeFeedbackRangeObject.Tick(d, targetSegment);
        }

        /// <summary>
        /// Calculates the firing target position by taking into account :
        /// 1/ The maximum range of the weapon holder <see cref="CoreInteractiveObject.GetFiredProjectileMaxRange"/>
        /// 2/ If there is an interactiveobject that is currently targetted <see cref="TargettableInteractiveObjectSelectionManager.GetCurrentlyTargettedInteractiveObject"/>
        /// the firing target position is adjuste to it's <see cref="CoreInteractiveObject.GetFiringTargetLocalPosition"/> if the targetted <see cref="CoreInteractiveObject"/> is in range.
        /// </summary>
        private Segment CalculateFiringTargetPosition()
        {
            /// 1/ Calculate the TargetPointWorldPos by taking into account the CoreInteractiveObject.GetFiredProjectileMaxRange
            FiringProjectilePathCalculation.CalculateProjectilePath_Forward(this.PlayerInteractiveObject, out Vector3 SourcePointWorldPos, out Quaternion ForwardProjectileRotation);
            Vector3 TargetPointWorldPos = SourcePointWorldPos +
                                          (ForwardProjectileRotation * Vector3.forward * this.PlayerInteractiveObject.GetFiredProjectileMaxRange());

            /// 2/ When an interactive object is targetter, the TargetPointWorldPos is adjusted if he is in range.
            if (this.TargettableInteractiveObjectSelectionManager.IsCurrentlyTargetting())
            {
                var currentTarget = this.TargettableInteractiveObjectSelectionManager.GetCurrentlyTargettedInteractiveObject();
                var currentTargetWorldFiringTargetPosition = currentTarget.InteractiveGameObject.GetLocalToWorld().MultiplyPoint(currentTarget.GetFiringTargetLocalPosition());
                if (Vector3.Distance(currentTargetWorldFiringTargetPosition, SourcePointWorldPos) <= this.PlayerInteractiveObject.GetFiredProjectileMaxRange())
                {
                    TargetPointWorldPos = currentTargetWorldFiringTargetPosition;
                }
            }

            return new Segment(SourcePointWorldPos, TargetPointWorldPos);
        }

        public void Dispose()
        {
            this._firingRangeFeedbackRangeObject.Destroy();
        }
    }


    /// <summary>
    /// Positioning <see cref="FiringRangeFeedbackRangeObject"/> on the segment provided by <see cref="FiringRangeFeedbackSystem.CalculateFiringTargetPosition"/>.
    /// The final positioning is calculated by raycasting to InteractiveObjects that can interacts with the projectile.
    /// </summary>
    class FiringRangeFeedbackRangeObject : CoreInteractiveObject
    {
        private const float BoxWidth = 0.1f;

        /// <summary>
        /// A <see cref="BoxRangeObjectV2"/> is used to avoid using <see cref="Physics.RaycastAll"/> that imply memory allocation every frame.
        /// Instead, the <see cref="FiringRangeFeedbackRangeObject"/> will create a <see cref="BoxRangeObjectV2"/>  
        /// </summary>
        private BoxRayRangeObject PlayerFiringRangeTriggerV2;

        /// <summary>
        /// The listener attached to <see cref="PlayerFiringRangeTrigger"/>
        /// </summary>
        private InteractiveObjectPhysicsEventListenerDelegated PlayerFiringRangeTriggerPhysicsListener;

        public FiringRangeFeedbackRangeObject(IInteractiveGameObject IInteractiveGameObject,
            GameObject FiredProjectileFeedbackPrefab, CoreInteractiveObject WeaponHolder)
        {
            var firedProjectileModel = MonoBehaviour.Instantiate(FiredProjectileFeedbackPrefab, IInteractiveGameObject.InteractiveGameObjectParent.transform);
            firedProjectileModel.transform.ResetLocalPositionAndRotation();
            BaseInit(IInteractiveGameObject, IsUpdatedInMainManager: false);

            this.PlayerFiringRangeTriggerV2 = new BoxRayRangeObject(IInteractiveGameObject.InteractiveGameObjectParent,
                new BoxRangeObjectInitialization()
                {
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    BoxRangeTypeDefinition = new BoxRangeTypeDefinition()
                    {
                        Center = new Vector3(0, 0, 0),
                        Size = new Vector3(BoxWidth, BoxWidth, BoxWidth)
                    }
                },
                this,
                BoxWidth,
                delegate(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo) { return FiredProjectile.FiredProjectileHasTriggerEnter_ShouldItBeDestroyed(interactiveObjectPhysicsTriggerInfo.OtherInteractiveObject, WeaponHolder); }
            );
        }

        public void Tick(float d, Segment segment)
        {
            base.Tick(d);

            /// target point may not be obstructed by obstacles or other objects, so we ray cast to ensure this is not the case

            var adjustedSegment = AdjustedSegment(segment);

            var gameObjectTransform = this.InteractiveGameObject.InteractiveGameObjectParent.transform;
            gameObjectTransform.transform.position = adjustedSegment.Target;
            gameObjectTransform.rotation = Quaternion.LookRotation((adjustedSegment.Target - adjustedSegment.Source).normalized);
            this.PlayerFiringRangeTriggerV2.UpdateRayDimensions(adjustedSegment.Distance, true);
        }

        /// <summary>
        /// Adjust the <paramref name="segment"/> <see cref="Segment.Target"/> by raycasting in all <see cref="InsideInteractiveObjects"/>. 
        /// </summary>
        private Segment AdjustedSegment(Segment segment)
        {
            Segment adjustedSegment = segment;
            var ray = new Ray(segment.Source, (segment.Target - segment.Source).normalized);
            foreach (var insideInteractiveObject in this.PlayerFiringRangeTriggerV2.InsideInteractiveObjects)
            {
                if (insideInteractiveObject.InteractiveGameObject.LogicCollider != null && insideInteractiveObject.InteractiveGameObject.LogicCollider.Raycast(ray, out RaycastHit hit, segment.Distance))
                {
                    if (Vector3.Distance(adjustedSegment.Source, hit.point) < adjustedSegment.Distance)
                    {
                        adjustedSegment = new Segment(adjustedSegment.Source, hit.point);
                    }
                }
            }

            return adjustedSegment;
        }

        public override void Destroy()
        {
            this.PlayerFiringRangeTriggerV2.OnDestroy();
            base.Destroy();
        }

        public override void Init()
        {
        }
    }
}