using System;
using System.Collections.Generic;
using InteractiveObjects_Interfaces;
using RangeObjects;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class SightObjectSystem : AInteractiveObjectSystem
    {
        [VE_Array] private List<CoreInteractiveObject> currentlyIntersectedInteractiveObjects = new List<CoreInteractiveObject>();

        private Action<CoreInteractiveObject> OnSightObjectSystemJustIntersected;
        private Action<CoreInteractiveObject> OnSightObjectSystemNoMoreIntersected;
        [DrawNested] private RangeObjectV2 SightRange;

        public SightObjectSystem(CoreInteractiveObject AssocaitedInteractiveObject, SightObjectSystemDefinition SightObjectSystemDefinition, InteractiveObjectTag PhysicsTagEventGuard,
            Action<CoreInteractiveObject> OnSightObjectSystemJustIntersected,
            Action<CoreInteractiveObject> OnSightObjectSystemIntersectedNothing,
            Action<CoreInteractiveObject> OnSightObjectSystemNoMoreIntersected)
        {
            this.OnSightObjectSystemJustIntersected = OnSightObjectSystemJustIntersected;
            this.OnSightObjectSystemNoMoreIntersected = OnSightObjectSystemNoMoreIntersected;

            SightRange = new RoundedFrustumRangeObjectV2(AssocaitedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent, new RoundedFrustumRangeObjectInitialization
                {
                    IsTakingIntoAccountObstacles = true,
                    RangeTypeID = RangeTypeID.SIGHT_VISION,
                    RoundedFrustumRangeTypeDefinition = new RoundedFrustumRangeTypeDefinition
                    {
                        FrustumV2 = SightObjectSystemDefinition.Frustum
                    }
                }, AssocaitedInteractiveObject, AssocaitedInteractiveObject.InteractiveGameObject.InteractiveGameObjectParent.name + "_SightObject");
            SightRange.RegisterIntersectionEventListener(new RangeIntersectionV2Listener_Delegated(SightRange, PhysicsTagEventGuard,
                OnJustIntersectedAction: OnSightJustIntersected, OnInterestedNothingAction: OnSightObjectSystemIntersectedNothing, OnJustNotIntersectedAction: OnSightNoMoreIntersected));
        }

        public List<CoreInteractiveObject> CurrentlyIntersectedInteractiveObjects => currentlyIntersectedInteractiveObjects;

        private void OnSightJustIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            currentlyIntersectedInteractiveObjects.Add(IntersectedInteractiveObject);
            if (OnSightObjectSystemJustIntersected != null) OnSightObjectSystemJustIntersected.Invoke(IntersectedInteractiveObject);
        }

        private void OnSightNoMoreIntersected(CoreInteractiveObject IntersectedInteractiveObject)
        {
            currentlyIntersectedInteractiveObjects.Remove(IntersectedInteractiveObject);
            if (OnSightObjectSystemNoMoreIntersected != null) OnSightObjectSystemNoMoreIntersected.Invoke(IntersectedInteractiveObject);
        }
    }
}