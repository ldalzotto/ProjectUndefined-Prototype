﻿using CoreGame;
using InteractiveObjects;
using InteractiveObjects_Interfaces;
using RangeObjects;
using SelectableObjects_Interfaces;
using UnityEngine;

namespace SelectableObject
{
    #region Callback Events

    public delegate void OnPlayerTriggerInSelectionEnterDelegate(CoreInteractiveObject IntersectedInteractiveObject);

    public delegate void OnPlayerTriggerInSelectionExitDelegate(CoreInteractiveObject IntersectedInteractiveObject);

    #endregion


    public class SelectableObjectSystem : AInteractiveObjectSystem, ISelectableObjectSystem
    {
        private CoreInteractiveObject AssociatedInteractiveObject;

        #region External Dependencies

        private SelectableObjectManagerV2 SelectableObjectManagerV2 = SelectableObjectManagerV2.Get();

        #endregion

        private RangeObjectV2 SphereRange;

        public SelectableObjectSystem(CoreInteractiveObject AssociatedInteractiveObject,
            SelectableObjectSystemDefinition SelectableObjectSystemDefinition, object AssociatedPlayerAction) //TODO -> replace object with RTPPlayerAction ref
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.AssociatedPlayerAction = AssociatedPlayerAction;
            SphereRange = new SphereRangeObjectV2(AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new SphereRangeObjectInitialization()
                {
                    RangeTypeID = RangeTypeID.NOT_DISPLAYED,
                    IsTakingIntoAccountObstacles = false,
                    SphereRangeTypeDefinition = new SphereRangeTypeDefinition
                    {
                        Radius = SelectableObjectSystemDefinition.SelectionRange
                    }
                }, AssociatedInteractiveObject, AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_SelectionRangeTrigger");
            SphereRange.RegisterPhysicsEventListener(new SelectableObjectPhysicsEventListener(OnPlayerTriggerInSelectionEnter, OnPlayerTriggerInSelectionExit));
        }

        //TODO -> replace object with RTPPlayerAction ref
        public object AssociatedPlayerAction { get; private set; }

        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return AssociatedInteractiveObject.InteractiveGameObject.AverageModelBounds;
        }

        public Transform GetTransform()
        {
            return AssociatedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.transform;
        }

        private void OnPlayerTriggerInSelectionEnter(CoreInteractiveObject IntersectedInteractiveObject)
        {
            SelectableObjectManagerV2.OnSelectableObjectEnter(this);
        }

        private void OnPlayerTriggerInSelectionExit(CoreInteractiveObject IntersectedInteractiveObject)
        {
            SelectableObjectManagerV2.RemoveInteractiveObjectFromSelectable(this);
        }

        public override void OnDestroy()
        {
            SphereRange.OnDestroy();
        }
    }

    internal class SelectableObjectPhysicsEventListener : ARangeObjectV2PhysicsEventListener
    {
        private InteractiveObjectTagStruct InteractiveObjectTagStruct;

        private OnPlayerTriggerInSelectionEnterDelegate OnPlayerTriggerInSelectionEnter;
        private OnPlayerTriggerInSelectionExitDelegate OnPlayerTriggerInSelectionExit;

        public SelectableObjectPhysicsEventListener(OnPlayerTriggerInSelectionEnterDelegate OnPlayerTriggerInSelectionEnter, OnPlayerTriggerInSelectionExitDelegate OnPlayerTriggerInSelectionExit)
        {
            InteractiveObjectTagStruct = new InteractiveObjectTagStruct {IsPlayer = 1};
            this.OnPlayerTriggerInSelectionEnter = OnPlayerTriggerInSelectionEnter;
            this.OnPlayerTriggerInSelectionExit = OnPlayerTriggerInSelectionExit;
        }

        public override bool ColliderSelectionGuard(RangeObjectPhysicsTriggerInfo RangeObjectPhysicsTriggerInfo)
        {
            return InteractiveObjectTagStruct.Compare(RangeObjectPhysicsTriggerInfo.OtherInteractiveObject.InteractiveObjectTag);
        }

        public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            OnPlayerTriggerInSelectionEnter.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }

        public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
        {
            OnPlayerTriggerInSelectionExit.Invoke(PhysicsTriggerInfo.OtherInteractiveObject);
        }
    }
}