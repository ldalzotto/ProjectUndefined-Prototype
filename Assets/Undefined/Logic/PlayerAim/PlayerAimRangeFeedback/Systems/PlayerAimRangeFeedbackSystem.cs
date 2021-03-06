﻿using System.Collections.Generic;
using CoreGame;
using FiredProjectile;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using RangeObjects;
using Targetting;
using UnityEngine;

namespace PlayerAim
{
    /// <summary>
    /// Spawns a <see cref="firingRangeFeedbackRangeObject"/> that will show where the Player is aiming at.
    /// This is visualized by the model of <see cref="firingRangeFeedbackRangeObject"/>.
    /// </summary>
    public struct PlayerAimRangeFeedbackSystem
    {
        public bool IsInitialized;

        private CoreInteractiveObject PlayerInteractiveObject;

        private FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef;
        private FiringRangeFeedbackRangeObject firingRangeFeedbackRangeObject;

        /// <summary>
        /// /!\ The creation of <see cref="PlayerAimRangeFeedbackSystem"/> must absolutely be done after that physics objects have been taken into account from the physics engine.
        ///     This is because the <paramref name="FiringPlayerActionTargetSystemRef"/> uses operations on newly created physics object to calculate
        ///     the target direction <see cref="FiringPlayerActionTargetSystem.Tick"/>
        /// </summary>
        public PlayerAimRangeFeedbackSystem(CoreInteractiveObject playerInteractiveObject, FiringPlayerActionTargetSystem FiringPlayerActionTargetSystemRef)
        {
            IsInitialized = true;
            PlayerInteractiveObject = playerInteractiveObject;
            this.FiringPlayerActionTargetSystemRef = FiringPlayerActionTargetSystemRef;
            this.firingRangeFeedbackRangeObject = new FiringRangeFeedbackRangeObject(
                InteractiveGameObjectFactory.Build_Allocate(new GameObject("FiringRangeFeedbackRangeObject")),
                PlayerAimingRangeFeedbackConfigurationGameObject.Get().playerAimingRangeVisualFeedbackConfiguration.FiredProjectileFeedbackPrefab,
                playerInteractiveObject);

            this.firingRangeFeedbackRangeObject.InitializeRaybox(CalculateFiringTargetPosition());
        }

        /// <summary>
        /// The <see cref="PlayerAimRangeFeedbackSystem"/> is updated after the player is updated because <see cref="PlayerObjectOrientationSystem"/> may apply player movement position constraints
        /// that are computed during player object update.
        /// The firing range feedback is thus calculated after player position has been updated. 
        /// </summary>
        public void AfterPlayerTick(float d)
        {
            if (this.IsInitialized)
            {
                var targetSegment = CalculateFiringTargetPosition();

                this.firingRangeFeedbackRangeObject.Tick(d, targetSegment);
            }
        }

        /// <summary>
        /// Calculates the firing target position by taking into account :
        /// 1/ The maximum range of the weapon holder <see cref="CoreInteractiveObject.GetFiredProjectileMaxRange"/>
        /// 2/ If there is an interactiveobject that is currently targetted <see cref="FiringLockSelectionManager.GetCurrentlyTargettedInteractiveObject"/>
        /// the firing target position is adjusted to it's <see cref="CoreInteractiveObject.GetFiringTargetLocalPosition"/> if the targetted <see cref="CoreInteractiveObject"/> is in range.
        /// </summary>
        private Segment CalculateFiringTargetPosition()
        {
            var SourcePointWorldPos = this.PlayerInteractiveObject.GetWeaponWorldFirePoint();
            Vector3 TargetPointWorldPos = SourcePointWorldPos +
                                          (FiringPlayerActionTargetSystemRef.TargetDirection * this.PlayerInteractiveObject.GetFiredProjectileMaxRange());
            return new Segment(SourcePointWorldPos, TargetPointWorldPos);
        }

        public void Dispose()
        {
            this.firingRangeFeedbackRangeObject?.Destroy();
        }
    }


    /// <summary>
    /// Positioning <see cref="FiringRangeFeedbackRangeObject"/> on the segment provided by <see cref="PlayerAimRangeFeedbackSystem.CalculateFiringTargetPosition"/>.
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
                delegate(InteractiveObjectPhysicsTriggerInfo interactiveObjectPhysicsTriggerInfo) { return FiredProjectile.FiredProjectileHasTriggerEnter_DealsDamage_And_MustBeDestroyed(interactiveObjectPhysicsTriggerInfo.OtherInteractiveObject, WeaponHolder); }
            );
        }

        /// <summary>
        /// At the first frame were the <see cref="FiringRangeFeedbackRangeObject"/> appears, it's <see cref="PlayerFiringRangeTriggerV2"/> is not initialized with Physics colliders.
        /// To overcome this, we call <see cref="PlayerFiringRangeTriggerV2.ManuallyDetectInsideColliders"/> that manyally check overlaps around the box.
        /// /!\ That is why thi method must automatically called after the <see cref="FiringRangeFeedbackRangeObject"/> has been created.
        /// </summary>
        public void InitializeRaybox(Segment segment)
        {
            /// to update positions and box size
            this.Tick(0f, segment);
            this.PlayerFiringRangeTriggerV2.ManuallyDetectInsideColliders();

            /// to update positions and box size while taking into account
            /// the manually calculated inside colliders
            this.Tick(0f, segment);
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
            foreach (var insideInteractiveObject in this.PlayerFiringRangeTriggerV2.GetInsideInteractiveObjects())
            {
                if (insideInteractiveObject.InteractiveGameObject.LogicCollider.Raycast(ray, out RaycastHit hit, segment.Distance))
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