using System;
using System.Collections.Generic;
using InteractiveObjects_Interfaces;
using RangeObjects;
using UnityEngine;
using VisualFeedback;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class DisarmObjectSystem : AInteractiveObjectSystem
    {
        [VE_Nested] private DisarmSystemDefinition DisarmSystemDefinition;

        private RangeObjectV2 SphereRange;

        public DisarmObjectSystem(CoreInteractiveObject AssociatedInteractiveObject, DisarmSystemDefinition DisarmObjectInitializationData,
            InteractiveObjectTag PhysicsEventListenerGuard, Action<CoreInteractiveObject> OnAssociatedDisarmObjectTriggerEnter,
            Action<CoreInteractiveObject> OnAssociatedDisarmObjectTriggerExit)
        {
            DisarmSystemDefinition = DisarmObjectInitializationData;
            SphereRange = new SphereRangeObjectV2(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization
                {
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                    {
                        Radius = DisarmObjectInitializationData.DisarmRange
                    }
                }, AssociatedInteractiveObject, AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_DisarmTriggerRange");
            SphereRange.RegisterPhysicsEventListener(new RangeObjectV2PhysicsEventListener_Delegated(PhysicsEventListenerGuard, OnAssociatedDisarmObjectTriggerEnter, OnAssociatedDisarmObjectTriggerExit));

            ProgressBarGameObject = new GameObject("ProgressBar");
            ProgressBarGameObject.transform.parent = AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
            progressbar = ProgressBarGameObject.AddComponent<CircleFillBarType>();

            progressbar.Init(Camera.main);
            progressbar.transform.position = AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition + (Vector3.up * (AssociatedInteractiveObject.InteractiveGameObject.AverageModelBounds.Bounds.max.y));
            progressbar.gameObject.SetActive(false);

            elapsedTime = 0f;
        }

        public bool IsTimeElasped()
        {
            return elapsedTime >= DisarmSystemDefinition.DisarmTime;
        }

        private float GetDisarmPercentage01()
        {
            return elapsedTime / DisarmSystemDefinition.DisarmTime;
        }

        public override void Tick(float d)
        {
            if (progressbar.gameObject.activeSelf) progressbar.Tick(GetDisarmPercentage01());
            if (InteractiveObjectDisarmingThisObject.Count > 0)
                for (var i = 0; i < InteractiveObjectDisarmingThisObject.Count; i++)
                    IncreaseTimeElapsedBy(d);
        }

        public override void OnDestroy()
        {
            SphereRange.OnDestroy();
        }

        private void IncreaseTimeElapsedBy(float increasedTime)
        {
            elapsedTime += increasedTime;

            if (GetDisarmPercentage01() > 0 && !progressbar.gameObject.activeSelf) CircleFillBarType.EnableInstace(progressbar);
        }

        #region Internal Dependencies

        [VE_Ignore] private GameObject ProgressBarGameObject;

        [VE_Nested] private CircleFillBarType progressbar;

        #endregion

        #region State

        [VE_Array] private HashSet<CoreInteractiveObject> InteractiveObjectDisarmingThisObject = new HashSet<CoreInteractiveObject>();

        [WireCircle(R = 1f, G = 0f, B = 0f)] private float elapsedTime;

        #endregion

        #region External Events

        public void AddInteractiveObjectDisarmingThisObject(CoreInteractiveObject CoreInteractiveObject)
        {
            InteractiveObjectDisarmingThisObject.Add(CoreInteractiveObject);
        }

        public void RemoveInteractiveObjectDisarmingThisObject(CoreInteractiveObject CoreInteractiveObject)
        {
            InteractiveObjectDisarmingThisObject.Remove(CoreInteractiveObject);
        }

        #endregion
    }
}