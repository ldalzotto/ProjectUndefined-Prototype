using System.Collections.Generic;
using CoreGame;
using InteractiveObjects;
using Obstacle;

namespace RangeObjects
{
    /// <summary>
    /// Tracks and set state of current intersection between a <see cref="RangeObjectV2"/> as <see cref="this.AssociatedRangeObject"/>
    /// and a <see cref="CoreInteractiveObject"/> as <see cref="this.TrackedInteractiveObject"/>
    /// </summary>
    public class RangeIntersectionCalculator
    {
        private RangeObjectV2 AssociatedRangeObject;
        private BlittableTransformChangeListenerManager inRangeCollidersMovementChangeTracker;

        private BlittableTransformChangeListenerManager sightModuleMovementChangeTracker;

        public RangeIntersectionCalculator(RangeObjectV2 RangeObject, CoreInteractiveObject TrackedInteractiveObject)
        {
            this.AssociatedRangeObject = RangeObject;
            this.TrackedInteractiveObject = TrackedInteractiveObject;
            this.sightModuleMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.inRangeCollidersMovementChangeTracker = new BlittableTransformChangeListenerManager(true, true);
            this.RangeIntersectionCalculatorV2UniqueID = _rangeIntersectionCalculatorManager.OnRangeIntersectionCalculatorV2ManagerCreation(this);
            RangeIntersectionCalculationManagerV2.ManualCalculation(new List<RangeIntersectionCalculator>() {this}, forceCalculation: true);
        }

        public int RangeIntersectionCalculatorV2UniqueID { get; private set; }
        public CoreInteractiveObject TrackedInteractiveObject { get; private set; }
        public bool IsInside { get; private set; }

        public InterserctionOperationType LastInterserctionOperationType { get; private set; }

        // Updated from RangeIntersection Manager
        // Update the AssociatedRangeObject and the TrackedInteractiveObject transform tracker to check if they have moved since the last frame.
        public bool TickChangedPositions()
        {
            var TrackedInteractiveGameObjectTransform = this.TrackedInteractiveObject.InteractiveGameObject.GetTransform();
            this.sightModuleMovementChangeTracker.Tick(this.AssociatedRangeObject.RangeGameObjectV2.BoundingCollider.transform.position,
                this.AssociatedRangeObject.RangeGameObjectV2.BoundingCollider.transform.eulerAngles);
            this.inRangeCollidersMovementChangeTracker.Tick(TrackedInteractiveGameObjectTransform.WorldPosition, TrackedInteractiveGameObjectTransform.WorldRotationEuler);
            return this.inRangeCollidersMovementChangeTracker.TransformChangedThatFrame() ||
                   this.sightModuleMovementChangeTracker.TransformChangedThatFrame();
        }

        public InterserctionOperationType Tick()
        {
            InterserctionOperationType returnOperation = InterserctionOperationType.Nothing;
            var newInside = this.RangeIntersectionCalculationManagerV2.GetRangeIntersectionResult(this);
            if (this.IsInside && !newInside)
            {
                returnOperation = InterserctionOperationType.JustNotInteresected;
            }
            else if (!this.IsInside && newInside)
            {
                returnOperation = InterserctionOperationType.JustInteresected;
            }
            else if (this.IsInside && newInside)
            {
                returnOperation = InterserctionOperationType.IntersectedNothing;
            }

            this.IsInside = newInside;

            this.LastInterserctionOperationType = returnOperation;
            return this.LastInterserctionOperationType;
        }

        public ObstacleListenerSystem GetAssociatedObstacleListener()
        {
            return this.AssociatedRangeObject.GetObstacleListener();
        }

        public RangeObjectV2 GetAssociatedRangeObject()
        {
            return this.AssociatedRangeObject;
        }

        public RangeType GetAssociatedRangeObjectType()
        {
            return this.AssociatedRangeObject.RangeType;
        }

        public void OnDestroy()
        {
            this._rangeIntersectionCalculatorManager.OnRangeIntersectionCalculatorV2ManagerDestroyed(this);
        }

        #region External Dependencies

        private RangeIntersectionCalculatorManager _rangeIntersectionCalculatorManager = RangeIntersectionCalculatorManager.Get();
        private RangeIntersectionCalculationManagerV2 RangeIntersectionCalculationManagerV2 = RangeIntersectionCalculationManagerV2.Get();

        #endregion
    }

    public enum InterserctionOperationType
    {
        JustInteresected = 0,
        JustNotInteresected = 1,
        IntersectedNothing = 2,
        Nothing = 3
    }
}