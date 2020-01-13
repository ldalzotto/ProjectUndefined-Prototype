using System;
using System.Collections.Generic;
using InteractiveObjects;
using RangeObjects;

namespace ProjectileDeflection
{
    /// <summary>
    /// Creates the <see cref="SphereOverlapRangeObject"/> representing the range in which CoreItneractiveObject can be deflected.
    /// </summary>
    struct ObjectsInsideDeflectionRangeSystem
    {
        private CoreInteractiveObject associatedInteractiveObject;
        private ProjectileDeflectionTrackingInteractiveObjectActionInherentData _projectileDeflectionTrackingInteractiveObjectActionInherentData;

        /// <summary>
        /// /!\ The <see cref="SphereOverlapRangeObject"/> is created when <see cref="ObjectsInsideDeflectionRangeSystem"/> is constructed <see cref="ObjectsInsideDeflectionRangeSystem()"/>
        /// This is because the <see cref="ObjectsInsideDeflectionRangeSystem"/> belongs to an <see cref="AInteractiveObjectAction"/> and that actions are supposed to be destroyed when they are finished.
        /// </summary>
        private SphereOverlapRangeObject SphereOverlapRangeObject;

        #region callbacks

        private Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered;
        private Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered;

        #endregion

        public ObjectsInsideDeflectionRangeSystem(CoreInteractiveObject associatedInteractiveObject,
            ProjectileDeflectionTrackingInteractiveObjectActionInherentData projectileDeflectionTrackingInteractiveObjectActionInherentData,
            Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered = null, Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered = null)
        {
            this.associatedInteractiveObject = associatedInteractiveObject;
            this._projectileDeflectionTrackingInteractiveObjectActionInherentData = projectileDeflectionTrackingInteractiveObjectActionInherentData;
            this.SphereOverlapRangeObject = null;
            this.OnInteractiveObjectJusInsideAndFiltered = OnInteractiveObjectJusInsideAndFiltered;
            this.OnInteractiveObjectJustOutsideAndFiltered = OnInteractiveObjectJustOutsideAndFiltered;

            this.SphereOverlapRangeObject = new SphereOverlapRangeObject(associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization()
                {
                    name = "ProjectileDeflectionRange",
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition()
                    {
                        Radius = this._projectileDeflectionTrackingInteractiveObjectActionInherentData.ProjectileDetectionRadius
                    }
                }, associatedInteractiveObject, delegate(InteractiveObjectPhysicsTriggerInfo InteractiveObjectPhysicsTriggerInfo) { return InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsDealingDamage; },
                "ProjectileDeflectionRange", OnInteractiveObjectJusInsideAndFiltered: OnInteractiveObjectJusInsideAndFiltered, OnInteractiveObjectJustOutsideAndFiltered: OnInteractiveObjectJustOutsideAndFiltered);
            SetDeflectionRangeCenterAsLogicColliderCenter();
            this.SphereOverlapRangeObject.ManuallyDetectInsideColliders();
        }

        /// <summary>
        /// The center of the <see cref="SphereOverlapRangeObject"/> is set as the center of the logic collider of the <see cref="associatedInteractiveObject"/>.
        /// </summary>
        private void SetDeflectionRangeCenterAsLogicColliderCenter()
        {
            this.SphereOverlapRangeObject.RangeGameObjectV2.RangeGameObject.transform.position = this.associatedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition().GetWorldCenter();
        }

        public void Tick(float d)
        {
            this.SetDeflectionRangeCenterAsLogicColliderCenter();
            this.SphereOverlapRangeObject.Tick(d);
        }

        public void Destroy()
        {
            this.SphereOverlapRangeObject?.OnDestroy();
        }

        #region Data Retrieval

        public HashSet<CoreInteractiveObject> GetInsideDeflectableInteractiveObjects()
        {
            return this.SphereOverlapRangeObject.GetInsideInteractiveObjects();
        }

        #endregion
    }
}