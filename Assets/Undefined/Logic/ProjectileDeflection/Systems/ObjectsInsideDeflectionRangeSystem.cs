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
        private ProjectileDeflectionActorDefinition ProjectileDeflectionActorDefinitionRef;

        private SphereOverlapRangeObject SphereOverlapRangeObject;

        #region callbacks

        private Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered;
        private Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered;

        #endregion
        
        public ObjectsInsideDeflectionRangeSystem(CoreInteractiveObject associatedInteractiveObject,
            ProjectileDeflectionActorDefinition ProjectileDeflectionActorDefinition, 
            Action<CoreInteractiveObject> OnInteractiveObjectJusInsideAndFiltered = null, Action<CoreInteractiveObject> OnInteractiveObjectJustOutsideAndFiltered = null)
        {
            this.associatedInteractiveObject = associatedInteractiveObject;
            this.ProjectileDeflectionActorDefinitionRef = ProjectileDeflectionActorDefinition;
            this.SphereOverlapRangeObject = null;
            this.OnInteractiveObjectJusInsideAndFiltered = OnInteractiveObjectJusInsideAndFiltered;
            this.OnInteractiveObjectJustOutsideAndFiltered = OnInteractiveObjectJustOutsideAndFiltered;
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

        #region External Events

        public void OnLowHealthStarted()
        {
            this.SphereOverlapRangeObject = new SphereOverlapRangeObject(associatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization()
                {
                    name = "ProjectileDeflectionRange",
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition()
                    {
                        Radius = this.ProjectileDeflectionActorDefinitionRef.ProjectileDetectionRadius
                    }
                }, associatedInteractiveObject, delegate(InteractiveObjectPhysicsTriggerInfo InteractiveObjectPhysicsTriggerInfo) { return InteractiveObjectPhysicsTriggerInfo.GetOtherInteractiveObjectTag().IsDealingDamage; },
                "ProjectileDeflectionRange", OnInteractiveObjectJusInsideAndFiltered: OnInteractiveObjectJusInsideAndFiltered, OnInteractiveObjectJustOutsideAndFiltered: OnInteractiveObjectJustOutsideAndFiltered);
            SetDeflectionRangeCenterAsLogicColliderCenter();
            this.SphereOverlapRangeObject.ManuallyDetectInsideColliders();
        }

        public void OnLowHealthEnded()
        {
            this.Destroy();
        }

        #endregion


        #region Data Retrieval

        public List<CoreInteractiveObject> GetInsideDeflectableInteractiveObjects()
        {
            return this.SphereOverlapRangeObject.InsideInteractiveObjectsList;
        }

        #endregion
    }
}